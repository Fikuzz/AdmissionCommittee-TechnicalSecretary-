USE [Приемная комиссия]
GO
/****** Object:  Table [dbo].[Абитуриент]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Абитуриент](
	[IDАбитуриента] [int] IDENTITY(1,1) NOT NULL,
	[Фамилия] [nvarchar](20) NOT NULL,
	[Имя] [nvarchar](20) NOT NULL,
	[Отчество] [nvarchar](20) NULL,
	[Школа] [nvarchar](50) NOT NULL,
	[ЭкзаменационныйЛист] [nvarchar](10) NULL,
	[ГодОкончанияШколы] [int] NOT NULL,
	[ГражданинРБ] [bit] NULL,
	[Гражданство] [nvarchar](30) NULL,
	[Общежитие] [bit] NULL,
	[ГодПоступления] [int] NOT NULL,
	[IDПланаПриема] [int] NOT NULL,
	[МестоРаботы] [nvarchar](40) NULL,
	[Должность] [nvarchar](40) NULL,
	[Сирота] [bit] NULL,
	[ЦелевойДоговор] [bit] NULL,
	[АбитуриентЗачислен] [bit] NULL,
	[IDВладельца] [int] NOT NULL,
	[IDРедактора] [int] NULL,
	[Удалено] [bit] NULL,
	[ДатаСоздания] [datetime] NULL,
	[ДатаРедактирования] [datetime] NULL,
 CONSTRAINT [PK__Абитурие__998FBBCDBBD36977] PRIMARY KEY CLUSTERED 
(
	[IDАбитуриента] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Атестат]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Атестат](
	[IDАтестата] [int] IDENTITY(1,1) NOT NULL,
	[IDАбитуриента] [int] NOT NULL,
	[IDШкалыСтраны] [int] NOT NULL,
	[СерияАтестата] [nvarchar](50) NULL,
	[СреднийБалл] [float] NOT NULL,
	[ДесятибальнаяСистема] [float] NOT NULL,
 CONSTRAINT [PK__Атестат__7B32B3EB2167DC4B] PRIMARY KEY CLUSTERED 
(
	[IDАтестата] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ОценкиАтестата]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ОценкиАтестата](
	[IDОценкиАтестата] [int] IDENTITY(1,1) NOT NULL,
	[Балл] [int] NOT NULL,
	[Количество] [int] NOT NULL,
	[IDАтестата] [int] NOT NULL,
 CONSTRAINT [PK__ОценкиАт__DFFC47AD44A93D7B] PRIMARY KEY CLUSTERED 
(
	[IDОценкиАтестата] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[GetAbiturientData]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*SELECT * FROM GetAbiturientData*/
CREATE VIEW [dbo].[GetAbiturientData]
AS
SELECT        dbo.Абитуриент.IDАбитуриента, dbo.Абитуриент.Фамилия + ' ' + dbo.Абитуриент.Имя + ' ' + dbo.Абитуриент.Отчество AS ФИО, dbo.Абитуриент.Школа, dbo.Абитуриент.ГодОкончанияШколы, 
                         dbo.Абитуриент.АбитуриентЗачислен,
                             (SELECT        SUM(dbo.ОценкиАтестата.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата INNER JOIN
                                                         dbo.Атестат ON dbo.Атестат.IDАтестата = dbo.ОценкиАтестата.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = dbo.Атестат.IDАбитуриента) AND (dbo.ОценкиАтестата.Балл = 1)
                               GROUP BY dbo.ОценкиАтестата.Балл) AS Mark1,
                             (SELECT        SUM(ОценкиАтестата_9.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_9 INNER JOIN
                                                         dbo.Атестат AS Атестат_10 ON Атестат_10.IDАтестата = ОценкиАтестата_9.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_10.IDАбитуриента) AND (ОценкиАтестата_9.Балл = 2)
                               GROUP BY ОценкиАтестата_9.Балл) AS Mark2,
                             (SELECT        SUM(ОценкиАтестата_8.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_8 INNER JOIN
                                                         dbo.Атестат AS Атестат_9 ON Атестат_9.IDАтестата = ОценкиАтестата_8.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_9.IDАбитуриента) AND (ОценкиАтестата_8.Балл = 3)
                               GROUP BY ОценкиАтестата_8.Балл) AS Mark3,
                             (SELECT        SUM(ОценкиАтестата_7.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_7 INNER JOIN
                                                         dbo.Атестат AS Атестат_8 ON Атестат_8.IDАтестата = ОценкиАтестата_7.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_8.IDАбитуриента) AND (ОценкиАтестата_7.Балл = 4)
                               GROUP BY ОценкиАтестата_7.Балл) AS Mark4,
                             (SELECT        SUM(ОценкиАтестата_6.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_6 INNER JOIN
                                                         dbo.Атестат AS Атестат_7 ON Атестат_7.IDАтестата = ОценкиАтестата_6.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_7.IDАбитуриента) AND (ОценкиАтестата_6.Балл = 5)
                               GROUP BY ОценкиАтестата_6.Балл) AS Mark5,
                             (SELECT        SUM(ОценкиАтестата_5.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_5 INNER JOIN
                                                         dbo.Атестат AS Атестат_6 ON Атестат_6.IDАтестата = ОценкиАтестата_5.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_6.IDАбитуриента) AND (ОценкиАтестата_5.Балл = 6)
                               GROUP BY ОценкиАтестата_5.Балл) AS Mark6,
                             (SELECT        SUM(ОценкиАтестата_4.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_4 INNER JOIN
                                                         dbo.Атестат AS Атестат_5 ON Атестат_5.IDАтестата = ОценкиАтестата_4.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_5.IDАбитуриента) AND (ОценкиАтестата_4.Балл = 7)
                               GROUP BY ОценкиАтестата_4.Балл) AS Mark7,
                             (SELECT        SUM(ОценкиАтестата_3.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_3 INNER JOIN
                                                         dbo.Атестат AS Атестат_4 ON Атестат_4.IDАтестата = ОценкиАтестата_3.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_4.IDАбитуриента) AND (ОценкиАтестата_3.Балл = 8)
                               GROUP BY ОценкиАтестата_3.Балл) AS Mark8,
                             (SELECT        SUM(ОценкиАтестата_2.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_2 INNER JOIN
                                                         dbo.Атестат AS Атестат_3 ON Атестат_3.IDАтестата = ОценкиАтестата_2.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_3.IDАбитуриента) AND (ОценкиАтестата_2.Балл = 9)
                               GROUP BY ОценкиАтестата_2.Балл) AS Mark9,
                             (SELECT        SUM(ОценкиАтестата_1.Количество) AS Expr1
                               FROM            dbo.ОценкиАтестата AS ОценкиАтестата_1 INNER JOIN
                                                         dbo.Атестат AS Атестат_2 ON Атестат_2.IDАтестата = ОценкиАтестата_1.IDАтестата
                               WHERE        (dbo.Абитуриент.IDАбитуриента = Атестат_2.IDАбитуриента) AND (ОценкиАтестата_1.Балл = 10)
                               GROUP BY ОценкиАтестата_1.Балл) AS Mark10, ROUND(SUM(Атестат_1.СреднийБалл) / COUNT(*), 2) AS СреднийБалл, dbo.Абитуриент.ЭкзаменационныйЛист, dbo.Абитуриент.Удалено, 
                         ROUND(SUM(Атестат_1.ДесятибальнаяСистема) / COUNT(*), 2) AS СреднийБаллВДесятибальнойСистеме, dbo.Абитуриент.Сирота, dbo.Абитуриент.ЦелевойДоговор
FROM            dbo.Абитуриент LEFT OUTER JOIN
                         dbo.Атестат AS Атестат_1 ON dbo.Абитуриент.IDАбитуриента = Атестат_1.IDАбитуриента
GROUP BY dbo.Абитуриент.IDАбитуриента, dbo.Абитуриент.Фамилия, dbo.Абитуриент.Имя, dbo.Абитуриент.Отчество, dbo.Абитуриент.Школа, dbo.Абитуриент.ГодОкончанияШколы, dbo.Абитуриент.АбитуриентЗачислен, 
                         dbo.Абитуриент.ЭкзаменационныйЛист, dbo.Абитуриент.Удалено, dbo.Абитуриент.Сирота, dbo.Абитуриент.ЦелевойДоговор, Атестат_1.IDШкалыСтраны
GO
/****** Object:  Table [dbo].[КонтактныеДанные]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[КонтактныеДанные](
	[IDКонтактныеДанные] [int] IDENTITY(1,1) NOT NULL,
	[IDАбитуриента] [int] NOT NULL,
	[Сведения] [nvarchar](70) NOT NULL,
	[IDТипКонтакта] [int] NOT NULL,
 CONSTRAINT [PK__Контактн__985028F09B245D35] PRIMARY KEY CLUSTERED 
(
	[IDКонтактныеДанные] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Настройки]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Настройки](
	[IDНастройки] [int] IDENTITY(1,1) NOT NULL,
	[ГодПоступления] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDНастройки] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ПаспортныеДанные]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ПаспортныеДанные](
	[IDПаспортныеДанные] [int] IDENTITY(1,1) NOT NULL,
	[IDАбитуриента] [int] NOT NULL,
	[ДатаРождения] [date] NOT NULL,
	[Серия] [nvarchar](15) NULL,
	[НомерПаспорта] [nvarchar](15) NULL,
	[ДатаВыдачи] [date] NULL,
	[НаименованиеОргана] [nvarchar](50) NULL,
	[ИдентификационныйНомер] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[IDПаспортныеДанные] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Перевод]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Перевод](
	[IDПеревода] [int] IDENTITY(1,1) NOT NULL,
	[IDШкалы] [int] NOT NULL,
	[Значение] [float] NOT NULL,
	[ДесятибальноеЗначение] [float] NOT NULL,
 CONSTRAINT [PK__Перевод__E0371C5E79140FCB] PRIMARY KEY CLUSTERED 
(
	[IDПеревода] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ПланПриема]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ПланПриема](
	[IDПланПриема] [int] IDENTITY(1,1) NOT NULL,
	[ГодПоступления] [varchar](4) NOT NULL,
	[IDСпециальности] [int] NOT NULL,
	[IDФормаОбучения] [int] NOT NULL,
	[IDФинансирования] [int] NOT NULL,
	[Количество] [int] NOT NULL,
	[КоличествоЦелевыхМест] [int] NOT NULL,
	[ЦТ] [bit] NOT NULL,
	[КодСпециальности] [nchar](13) NOT NULL,
 CONSTRAINT [PK__ПланПрие__2595E66AC64EC603] PRIMARY KEY CLUSTERED 
(
	[IDПланПриема] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Пользователь]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Пользователь](
	[IDПользователя] [int] IDENTITY(1,1) NOT NULL,
	[Логин] [nvarchar](10) NOT NULL,
	[ФИО] [nvarchar](50) NOT NULL,
	[IDРоли] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDПользователя] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Роль]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Роль](
	[IDРоли] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDРоли] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[СертификатЦТ]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[СертификатЦТ](
	[IDСертификатаЦТ] [int] IDENTITY(1,1) NOT NULL,
	[IDАбитуриента] [int] NOT NULL,
	[Дисциплина] [nvarchar](20) NULL,
	[Балл] [int] NOT NULL,
	[ДесятибальноеЗначение] [float] NOT NULL,
	[ГодПрохождения] [int] NULL,
	[НомерСерии] [int] NULL,
 CONSTRAINT [PK__Сертифик__CD497088FCC4CBC1] PRIMARY KEY CLUSTERED 
(
	[IDСертификатаЦТ] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Специальность]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Специальность](
	[IDСпециальность] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](40) NOT NULL,
	[Буква] [char](1) NOT NULL,
 CONSTRAINT [PK__Специаль__67713123C471BC0F] PRIMARY KEY CLUSTERED 
(
	[IDСпециальность] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Статьи]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Статьи](
	[IDСтатьи] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](20) NULL,
	[Приоритет] [int] NOT NULL,
	[ПолноеНаименование] [nvarchar](50) NULL,
 CONSTRAINT [PK__Статьи__039CE28169CFC07B] PRIMARY KEY CLUSTERED 
(
	[IDСтатьи] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[СтатьиАбитуриента]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[СтатьиАбитуриента](
	[IDСтатьиАбитуриента] [int] IDENTITY(1,1) NOT NULL,
	[IDАбитуриента] [int] NOT NULL,
	[IDСтатьи] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDСтатьиАбитуриента] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[СтранаОбучения]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[СтранаОбучения](
	[IDСтраныОбучения] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](40) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDСтраныОбучения] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ТипКонтакта]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ТипКонтакта](
	[IDТипКонтакта] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDТипКонтакта] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Финансирование]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Финансирование](
	[IDФинансирования] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDФинансирования] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ФормаОбучения]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ФормаОбучения](
	[IDФормаОбучения] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](20) NOT NULL,
	[Образование] [nvarchar](40) NOT NULL,
 CONSTRAINT [PK__ФормаОбу__7BE297F5ABAB32BF] PRIMARY KEY CLUSTERED 
(
	[IDФормаОбучения] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Шкала]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Шкала](
	[IDШкалы] [int] IDENTITY(1,1) NOT NULL,
	[Наименование] [nvarchar](25) NOT NULL,
	[КоличествоБаллов] [int] NOT NULL,
 CONSTRAINT [PK__Шкала__A0AAEA358404A57D] PRIMARY KEY CLUSTERED 
(
	[IDШкалы] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ШкалаСтраны]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ШкалаСтраны](
	[IDШкалыСтраны] [int] IDENTITY(1,1) NOT NULL,
	[IDШкалы] [int] NOT NULL,
	[IDСтраныОбучения] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IDШкалыСтраны] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Абитуриент] ADD  CONSTRAINT [DF_Абитуриент_АбитуриентЗачислен]  DEFAULT ((0)) FOR [АбитуриентЗачислен]
GO
ALTER TABLE [dbo].[Абитуриент] ADD  CONSTRAINT [DF_Абитуриент_Удалено]  DEFAULT ((0)) FOR [Удалено]
GO
ALTER TABLE [dbo].[Абитуриент] ADD  CONSTRAINT [DF_Абитуриент_ДатаСоздания]  DEFAULT (getdate()) FOR [ДатаСоздания]
GO
ALTER TABLE [dbo].[Абитуриент]  WITH CHECK ADD  CONSTRAINT [FK__Абитуриен__IDВла__05D8E0BE] FOREIGN KEY([IDВладельца])
REFERENCES [dbo].[Пользователь] ([IDПользователя])
GO
ALTER TABLE [dbo].[Абитуриент] CHECK CONSTRAINT [FK__Абитуриен__IDВла__05D8E0BE]
GO
ALTER TABLE [dbo].[Абитуриент]  WITH CHECK ADD  CONSTRAINT [FK__Абитуриен__IDПла__04E4BC85] FOREIGN KEY([IDПланаПриема])
REFERENCES [dbo].[ПланПриема] ([IDПланПриема])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Абитуриент] CHECK CONSTRAINT [FK__Абитуриен__IDПла__04E4BC85]
GO
ALTER TABLE [dbo].[Абитуриент]  WITH CHECK ADD  CONSTRAINT [FK__Абитуриен__IDРед__06CD04F7] FOREIGN KEY([IDРедактора])
REFERENCES [dbo].[Пользователь] ([IDПользователя])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Абитуриент] CHECK CONSTRAINT [FK__Абитуриен__IDРед__06CD04F7]
GO
ALTER TABLE [dbo].[Атестат]  WITH CHECK ADD  CONSTRAINT [FK__Атестат__IDАбиту__65370702] FOREIGN KEY([IDАбитуриента])
REFERENCES [dbo].[Абитуриент] ([IDАбитуриента])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Атестат] CHECK CONSTRAINT [FK__Атестат__IDАбиту__65370702]
GO
ALTER TABLE [dbo].[Атестат]  WITH CHECK ADD  CONSTRAINT [FK__Атестат__IDШкалы__662B2B3B] FOREIGN KEY([IDШкалыСтраны])
REFERENCES [dbo].[ШкалаСтраны] ([IDШкалыСтраны])
GO
ALTER TABLE [dbo].[Атестат] CHECK CONSTRAINT [FK__Атестат__IDШкалы__662B2B3B]
GO
ALTER TABLE [dbo].[КонтактныеДанные]  WITH CHECK ADD  CONSTRAINT [FK__Контактны__IDАби__56E8E7AB] FOREIGN KEY([IDАбитуриента])
REFERENCES [dbo].[Абитуриент] ([IDАбитуриента])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[КонтактныеДанные] CHECK CONSTRAINT [FK__Контактны__IDАби__56E8E7AB]
GO
ALTER TABLE [dbo].[КонтактныеДанные]  WITH CHECK ADD  CONSTRAINT [FK__Контактны__IDТип__57DD0BE4] FOREIGN KEY([IDТипКонтакта])
REFERENCES [dbo].[ТипКонтакта] ([IDТипКонтакта])
GO
ALTER TABLE [dbo].[КонтактныеДанные] CHECK CONSTRAINT [FK__Контактны__IDТип__57DD0BE4]
GO
ALTER TABLE [dbo].[ОценкиАтестата]  WITH CHECK ADD  CONSTRAINT [FK__ОценкиАте__IDАте__690797E6] FOREIGN KEY([IDАтестата])
REFERENCES [dbo].[Атестат] ([IDАтестата])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ОценкиАтестата] CHECK CONSTRAINT [FK__ОценкиАте__IDАте__690797E6]
GO
ALTER TABLE [dbo].[ПаспортныеДанные]  WITH CHECK ADD  CONSTRAINT [FK__Паспортны__IDАби__5224328E] FOREIGN KEY([IDАбитуриента])
REFERENCES [dbo].[Абитуриент] ([IDАбитуриента])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ПаспортныеДанные] CHECK CONSTRAINT [FK__Паспортны__IDАби__5224328E]
GO
ALTER TABLE [dbo].[Перевод]  WITH CHECK ADD  CONSTRAINT [FK__Перевод__IDШкалы__5CA1C101] FOREIGN KEY([IDШкалы])
REFERENCES [dbo].[Шкала] ([IDШкалы])
GO
ALTER TABLE [dbo].[Перевод] CHECK CONSTRAINT [FK__Перевод__IDШкалы__5CA1C101]
GO
ALTER TABLE [dbo].[ПланПриема]  WITH CHECK ADD  CONSTRAINT [FK__ПланПрием__IDСпе__787EE5A0] FOREIGN KEY([IDСпециальности])
REFERENCES [dbo].[Специальность] ([IDСпециальность])
GO
ALTER TABLE [dbo].[ПланПриема] CHECK CONSTRAINT [FK__ПланПрием__IDСпе__787EE5A0]
GO
ALTER TABLE [dbo].[ПланПриема]  WITH CHECK ADD  CONSTRAINT [FK__ПланПрием__IDФин__7A672E12] FOREIGN KEY([IDФинансирования])
REFERENCES [dbo].[Финансирование] ([IDФинансирования])
GO
ALTER TABLE [dbo].[ПланПриема] CHECK CONSTRAINT [FK__ПланПрием__IDФин__7A672E12]
GO
ALTER TABLE [dbo].[ПланПриема]  WITH CHECK ADD  CONSTRAINT [FK__ПланПрием__IDФор__797309D9] FOREIGN KEY([IDФормаОбучения])
REFERENCES [dbo].[ФормаОбучения] ([IDФормаОбучения])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ПланПриема] CHECK CONSTRAINT [FK__ПланПрием__IDФор__797309D9]
GO
ALTER TABLE [dbo].[Пользователь]  WITH CHECK ADD  CONSTRAINT [FK__Пользоват__IDРол__02084FDA] FOREIGN KEY([IDРоли])
REFERENCES [dbo].[Роль] ([IDРоли])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Пользователь] CHECK CONSTRAINT [FK__Пользоват__IDРол__02084FDA]
GO
ALTER TABLE [dbo].[СертификатЦТ]  WITH CHECK ADD  CONSTRAINT [FK__Сертифика__IDАби__4F47C5E3] FOREIGN KEY([IDАбитуриента])
REFERENCES [dbo].[Абитуриент] ([IDАбитуриента])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[СертификатЦТ] CHECK CONSTRAINT [FK__Сертифика__IDАби__4F47C5E3]
GO
ALTER TABLE [dbo].[СтатьиАбитуриента]  WITH CHECK ADD  CONSTRAINT [FK__СтатьиАби__IDАби__1BC821DD] FOREIGN KEY([IDАбитуриента])
REFERENCES [dbo].[Абитуриент] ([IDАбитуриента])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[СтатьиАбитуриента] CHECK CONSTRAINT [FK__СтатьиАби__IDАби__1BC821DD]
GO
ALTER TABLE [dbo].[СтатьиАбитуриента]  WITH CHECK ADD  CONSTRAINT [FK__СтатьиАби__IDСта__1CBC4616] FOREIGN KEY([IDСтатьи])
REFERENCES [dbo].[Статьи] ([IDСтатьи])
GO
ALTER TABLE [dbo].[СтатьиАбитуриента] CHECK CONSTRAINT [FK__СтатьиАби__IDСта__1CBC4616]
GO
ALTER TABLE [dbo].[ШкалаСтраны]  WITH CHECK ADD FOREIGN KEY([IDСтраныОбучения])
REFERENCES [dbo].[СтранаОбучения] ([IDСтраныОбучения])
GO
ALTER TABLE [dbo].[ШкалаСтраны]  WITH CHECK ADD  CONSTRAINT [FK__ШкалаСтра__IDШка__6166761E] FOREIGN KEY([IDШкалы])
REFERENCES [dbo].[Шкала] ([IDШкалы])
GO
ALTER TABLE [dbo].[ШкалаСтраны] CHECK CONSTRAINT [FK__ШкалаСтра__IDШка__6166761E]
GO
/****** Object:  StoredProcedure [dbo].[AbiturientsPriority]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[AbiturientsPriority]
@ID INT
AS
BEGIN
	SELECT IDАбитуриента, Фамилия, Имя, Отчество, Сирота, ЦелевойДоговор,
		(SELECT CASE WHEN MIN(Приоритет) IS NULL THEN 10 ELSE MIN(Приоритет) END 
		FROM СтатьиАбитуриента JOIN Статьи ON (СтатьиАбитуриента.IDСтатьи = Статьи.IDСтатьи) 
		WHERE Абитуриент.IDАбитуриента = СтатьиАбитуриента.IDАбитуриента) AS ПриоритетАбитуриента,
		(SELECT ROUND(MAX(ДесятибальнаяСистема),2) FROM Атестат WHERE Атестат.IDАбитуриента = Абитуриент.IDАбитуриента) AS [Средний балл], Удалено
	FROM Абитуриент
	WHERE IDПланаПриема = @ID
	ORDER BY Удалено, Сирота DESC, ЦелевойДоговор DESC, ПриоритетАбитуриента, [Средний балл] DESC
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Abiturient]    Script Date: 10.05.2022 15:47:54 ******/
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
	INSERT INTO Абитуриент(Фамилия, Имя, Отчество, Школа, ГодОкончанияШколы, ГражданинРБ, Гражданство, 
						   Общежитие, ГодПоступления, IDПланаПриема, МестоРаботы, Должность, Сирота, 
						   ЦелевойДоговор, АбитуриентЗачислен, IDВладельца, Удалено, ДатаСоздания, ЭкзаменационныйЛист) 
	OUTPUT inserted.IDАбитуриента 
	VALUES(@surename, @name, @otchestvo, @shool, @graduationYear, @grajdanstvoRB, @grajdanstvo, @obshejitie, YEAR(GETDATE()), @planPriema, @workPlace, @doljnost, @sirota, @dogovor, 0, @user, 0,  GETDATE(), @ExamList)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Atestat]    Script Date: 10.05.2022 15:47:54 ******/
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
IF (SELECT ДесятибальноеЗначение FROM Перевод WHERE IDШкалы = (SELECT IDШкалы FROM Шкала WHERE Наименование = @scaleName) AND Значение = ROUND(@avgMarks,1)) IS NOT NULL
	INSERT INTO Атестат (IDАбитуриента, IDШкалыСтраны, СерияАтестата, СреднийБалл, ДесятибальнаяСистема) 
		OUTPUT inserted.IDАтестата 
		VALUES(@abiturient,(SELECT IDШкалыСтраны FROM ШкалаСтраны JOIN Шкала ON (ШкалаСтраны.IDШкалы = Шкала.IDШкалы) WHERE Наименование LIKE @scaleName), @attestatSeries,
		ROUND(@avgMarks,2),(SELECT ДесятибальноеЗначение FROM Перевод WHERE IDШкалы = (SELECT IDШкалы FROM Шкала WHERE Наименование = @scaleName) AND Значение = ROUND(@avgMarks,1)))
ELSE
	INSERT INTO Атестат (IDАбитуриента, IDШкалыСтраны, СерияАтестата, СреднийБалл, ДесятибальнаяСистема) 
		OUTPUT inserted.IDАтестата 
		VALUES(@abiturient,(SELECT IDШкалыСтраны FROM ШкалаСтраны JOIN Шкала ON (ШкалаСтраны.IDШкалы = Шкала.IDШкалы) WHERE Наименование LIKE @scaleName), @attestatSeries,
		ROUND(@avgMarks,2),@avgMarks)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_ContctData]    Script Date: 10.05.2022 15:47:54 ******/
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
INSERT INTO КонтактныеДанные(IDАбитуриента, Сведения, IDТипКонтакта) 
	VALUES(@abiturient,@svedeniya,
			(SELECT IDТипКонтакта FROM ТипКонтакта WHERE Наименование = @contactType))
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Mark]    Script Date: 10.05.2022 15:47:54 ******/
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
	INSERT INTO ОценкиАтестата (Балл, Количество, IDАтестата) VALUES (@mark,@colvo,@attestat)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_PassportData]    Script Date: 10.05.2022 15:47:54 ******/
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
	INSERT INTO ПаспортныеДанные (IDАбитуриента,ДатаВыдачи,ДатаРождения,Серия,НомерПаспорта,НаименованиеОргана,ИдентификационныйНомер) 
	VALUES(@abiturient,@dateIssue,@dateOfBirth,@series,@PasspornNum,@name,@identNum)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_PlanPriema]    Script Date: 10.05.2022 15:47:54 ******/
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
@CT bit,
@Code varchar(13)
AS
BEGIN
	INSERT INTO ПланПриема(ГодПоступления, IDСпециальности, IDФормаОбучения, IDФинансирования, Количество, КоличествоЦелевыхМест, ЦТ, КодСпециальности) VALUES(
		@year,
		(SELECT IDСпециальность FROM Специальность WHERE Наименование = @spec),
		(SELECT IDФормаОбучения FROM ФормаОбучения WHERE Наименование = @form AND Образование = @obr),
		(SELECT IDФинансирования FROM Финансирование WHERE Наименование = @fin),
		@kolva,
		@kolvaCel,
		@CT,
		@Code
	)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Sertificat]    Script Date: 10.05.2022 15:47:54 ******/
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
	INSERT INTO СертификатЦТ (IDАбитуриента, Дисциплина, Балл, ДесятибальноеЗначение, ГодПрохождения, НомерСерии) 
	VALUES(@sertificat,@disciplin,@mark,@decMark,@year,@serialNum)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_Stati]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Add_Stati]
@abiturient int,
@statya int
AS
BEGIN
	INSERT INTO СтатьиАбитуриента(IDАбитуриента,IDСтатьи) VALUES(@abiturient,@statya)
END
GO
/****** Object:  StoredProcedure [dbo].[Add_User]    Script Date: 10.05.2022 15:47:54 ******/
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
INSERT INTO Пользователь(Логин, ФИО, IDРоли) 
		OUTPUT inserted.IDПользователя
		VALUES (@login, @fio, (SElECT IDРоли FROM Роль WHERE Наименование = @role))
END
GO
/****** Object:  StoredProcedure [dbo].[Del_AbiturientMarks]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Del_AbiturientMarks]
@abiturient int
AS
BEGIN
	DECLARE @lastID int = 0;
	select @lastID = MAX(IDОценкиАтестата) FROM ОценкиАтестата;
	DELETE ОценкиАтестата WHERE (IDАтестата = ALL(SELECT IDАтестата FROM Атестат WHERE IDАбитуриента = @abiturient))
	UPDATE Абитуриент SET АбитуриентЗачислен = 0,
						  Удалено = 1
					WHERE IDАбитуриента = @abiturient;
	DBCC CHECKIDENT (ОценкиАтестата, RESEED, @lastID);
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaAttestat]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaAttestat]
@abiturient int
AS
BEGIN
	SELECT IDАтестата, СерияАтестата as Num,
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 1) as [n1],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 2) as [n2],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 3) as [n3],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 4) as [n4],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 5) as [n5],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 6) as [n6],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 7) as [n7],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 8) as [n8],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 9) as [n9],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 10) as [n10],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 11) as [n11],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 12) as [n12],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 13) as [n13],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 14) as [n14],
	(SELECT Количество FROM ОценкиАтестата WHERE ОценкиАтестата.IDАтестата = Атестат.IDАтестата AND балл = 15) as [n15],
	(SELECT Наименование FROM Шкала WHERE Шкала.IDШкалы = (SELECT ШкалаСтраны.IDШкалы FROM ШкалаСтраны WHERE Атестат.IDШкалыСтраны = IDШкалыСтраны)) as [Scale], 
	ROUND (СреднийБалл, 2, 1) as СреднийБалл,ROUND (ДесятибальнаяСистема, 2, 1) as ДесятибальнаяСистема FROM Атестат WHERE IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaFullInfo]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaFullInfo]
@abiturient int
AS
BEGIN
	SELECT Фамилия + ' ' + Имя + ' ' + Отчество, Школа, ГодОкончанияШколы, ДатаРождения, ДатаВыдачи, Серия, НомерПаспорта,НаименованиеОргана,ИдентификационныйНомер, Гражданство, МестоРаботы, Должность, (SELECT ФИО FROM Пользователь WHERE IDПользователя = IDВладельца), (SELECT ФИО FROM Пользователь WHERE IDПользователя = IDРедактора), ДатаСоздания, ДатаРедактирования, АбитуриентЗачислен, Удалено
	FROM Абитуриент LEFT JOIN ПаспортныеДанные ON (Абитуриент.IDАбитуриента = ПаспортныеДанные.IDАбитуриента) 
	WHERE Абитуриент.IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaKontakti]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaKontakti]
@abiturient int
AS
BEGIN
	SELECT IDКонтактныеДанные, ROW_NUMbER() OVER(ORDER BY IDКонтактныеДанные) as Num, (SELECT Наименование FROM ТипКонтакта WHERE КонтактныеДанные.IDТипКонтакта = ТипКонтакта.IDТипКонтакта) as [ТипКонтакта], Сведения 
	FROM  КонтактныеДанные 
	WHERE IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientaSertificati]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientaSertificati]
@abiturient int
AS
BEGIN
	SELECT IDСертификатаЦТ, НомерСерии as num, Дисциплина, ГодПрохождения, Балл, ДесятибальноеЗначение 
	FROM СертификатЦТ 
	WHERE IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientList]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientList]
@PlanPriema int
AS
BEGIN
SELECT Абитуриент.IDАбитуриента,  Абитуриент.Фамилия + ' ' + Абитуриент.Имя +' '+Абитуриент.Отчество AS ФИО,
Абитуриент.Сирота, Абитуриент.ЦелевойДоговор, Пользователь.ФИО AS Владелец, ДатаСоздания, АбитуриентЗачислен, Удалено 
	FROM Абитуриент LEFT JOIN Пользователь ON(Абитуриент.IDВладельца = Пользователь.IDПользователя) 
	WHERE Абитуриент.IDПланаПриема = @PlanPriema
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientMainInfo]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientMainInfo]
@abiturient int
AS
BEGIN
	SELECT Фамилия, Имя, Отчество, Школа, ГодОкончанияШколы, ДатаРождения, ДатаВыдачи, Серия, НомерПаспорта,НаименованиеОргана,ИдентификационныйНомер, Гражданство, МестоРаботы, Должность, Общежитие, Сирота, ЦелевойДоговор, ЭкзаменационныйЛист 
	FROM Абитуриент LEFT JOIN ПаспортныеДанные ON (Абитуриент.IDАбитуриента = ПаспортныеДанные.IDАбитуриента) 
	WHERE Абитуриент.IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Get_AbiturientPriority]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_AbiturientPriority]
AS
BEGIN
	SELECT Фамилия, Имя, Отчество, Сирота, ЦелевойДоговор,
		(SELECT CASE WHEN MIN(Приоритет) IS NULL THEN 10 ELSE MIN(Приоритет) END 
		FROM СтатьиАбитуриента JOIN Статьи ON (СтатьиАбитуриента.IDСтатьи = Статьи.IDСтатьи) 
		WHERE Абитуриент.IDАбитуриента = СтатьиАбитуриента.IDАбитуриента) AS ПриоритетАбитуриента,
		(SELECT ROUND(MAX(ДесятибальнаяСистема),2) FROM Атестат WHERE Атестат.IDАбитуриента = Абитуриент.IDАбитуриента) AS [Средний балл]
	FROM Абитуриент
	ORDER BY ПриоритетАбитуриента, [Средний балл] DESC
END
GO
/****** Object:  StoredProcedure [dbo].[Get_MarkConvert]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_MarkConvert]
@scaleName varchar(50),
@mark int
AS
BEGIN
	SELECT ДесятибальноеЗначение FROM Перевод WHERE IDШкалы = (SELECT IDШкалы FROM Шкала WHERE Наименование = @scaleName) AND Значение = @mark
END
GO
/****** Object:  StoredProcedure [dbo].[Get_PlaniPriema]    Script Date: 10.05.2022 15:47:54 ******/
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
SELECT ГодПоступления, Специальность.Наименование, Финансирование.Наименование, ФормаОбучения.Наименование, Образование, ПланПриема.IDПланПриема, (SELECT COUNT(*) FROM Абитуриент WHERE IDПланаПриема = ПланПриема.IDПланПриема), КодСпециальности
	FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) 
		JOIN Финансирование ON(ПланПриема.IDФинансирования = Финансирование.IDФинансирования) 
		JOIN ФормаОбучения ON(ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения) 
	WHERE Специальность.Наименование = @specialost
		AND (Финансирование.Наименование like @budjet OR Финансирование.Наименование like @hozrash)
		AND (Образование like @bazovoe OR Образование like @srednee) 
		AND (ФормаОбучения.Наименование like @dnevnaya OR ФормаОбучения.Наименование like @zaochnaya)
END
GO
/****** Object:  StoredProcedure [dbo].[Get_PlanPrieaByID]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Get_PlanPrieaByID]
@id int
AS
BEGIN
SELECT IDСпециальности, IDФормаОбучения, IDФинансирования, Количество, КоличествоЦелевыхМест, ГодПоступления, КодСпециальности, 
	(SELECT Наименование FROM Специальность WHERE Специальность.IDСпециальность = ПланПриема.IDСпециальности), 
	(SELECT Наименование FROM ФормаОбучения WHERE ФормаОбучения.IDФормаОбучения = ПланПриема.IDФормаОбучения), 
	(SELECT Образование FROM ФормаОбучения WHERE ФормаОбучения.IDФормаОбучения = ПланПриема.IDФормаОбучения),
	(SELECT Наименование FROM Финансирование WHERE Финансирование.IDФинансирования = ПланПриема.IDФинансирования), ЦТ 
FROM ПланПриема WHERE IDПланПриема = @id
End
GO
/****** Object:  StoredProcedure [dbo].[Get_PlanPrieaBySpeciality]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Get_PlanPrieaBySpeciality]
@spec nvarchar(50)
AS
BEGIN
SELECT IDПланПриема, IDСпециальности, IDФормаОбучения, IDФинансирования, Количество, КоличествоЦелевыхМест, ГодПоступления, КодСпециальности, 
	(SELECT Наименование FROM Специальность WHERE Специальность.IDСпециальность = ПланПриема.IDСпециальности), 
	(SELECT Наименование FROM ФормаОбучения WHERE ФормаОбучения.IDФормаОбучения = ПланПриема.IDФормаОбучения), 
	(SELECT Образование FROM ФормаОбучения WHERE ФормаОбучения.IDФормаОбучения = ПланПриема.IDФормаОбучения),
	(SELECT Наименование FROM Финансирование WHERE Финансирование.IDФинансирования = ПланПриема.IDФинансирования), ЦТ 
FROM ПланПриема WHERE (SELECT Наименование FROM Специальность WHERE Специальность.IDСпециальность = ПланПриема.IDСпециальности) = @spec
End
GO
/****** Object:  StoredProcedure [dbo].[Get_PlanPriemaID]    Script Date: 10.05.2022 15:47:54 ******/
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
	SELECT IDПланПриема FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) 
										JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения) 
										JOIN Финансирование ON (ПланПриема.IDФинансирования = Финансирование.IDФинансирования) 
	WHERE Специальность.Наименование LIKE @speciality AND 
		  ФормаОбучения.Наименование LIKE @formOfEducation AND 
		  Финансирование.Наименование LIKE @financing AND 
		  ФормаОбучения.Образование LIKE @education
END
GO
/****** Object:  StoredProcedure [dbo].[Get_SpecialnostiName]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_SpecialnostiName]
@useFilter bit
AS
BEGIN
	SELECT Наименование FROM Специальность WHERE IDСпециальность = ANY (SELECT IDСпециальности FROM ПланПриема) OR @useFilter = 0
END
GO
/****** Object:  StoredProcedure [dbo].[Get_StatiAbiturienta]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_StatiAbiturienta]
@abiturient int
AS
BEGIN
	SELECT Наименование FROM Статьи JOIN СтатьиАбитуриента ON (Статьи.IDСтатьи = СтатьиАбитуриента.IDСтатьи) WHERE СтатьиАбитуриента.IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[GetAbiturientCountForStats]    Script Date: 10.05.2022 15:47:54 ******/
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
	SELECT ROUND(AVG(ДесятибальнаяСистема),1)
		FROM Абитуриент JOIN Атестат ON (Абитуриент.IDАбитуриента = Атестат.IDАбитуриента)
		WHERE Абитуриент.IDПланаПриема = @IDPlanPriema 
			and Абитуриент.IDАбитуриента != all(SELECT IDАбитуриента FROM Абитуриент WHERE Абитуриент.IDПланаПриема = @IDPlanPriema AND Абитуриент.ЦелевойДоговор = 1)
			and Абитуриент.IDАбитуриента != all((SELECT IDАбитуриента FROM Абитуриент WHERE Абитуриент.IDПланаПриема = @IDPlanPriema AND (Абитуриент.Сирота = 1 OR (SELECT MIN(Приоритет) FROM СтатьиАбитуриента JOIN Статьи ON (СтатьиАбитуриента.IDСтатьи = Статьи.IDСтатьи) WHERE Абитуриент.IDАбитуриента = IDАбитуриента)= 0)))
		GROUP BY Абитуриент.IDАбитуриента
		HAVING ROUND(AVG(ДесятибальнаяСистема),1) >= @minMark AND ROUND(AVG(ДесятибальнаяСистема),1) <= @maxMark
END
GO
/****** Object:  StoredProcedure [dbo].[GetStats]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[GetStats]
@spec varchar(50)
AS
BEGIN
SELECT ПланПриема.IDПланПриема,
	   CASE WHEN (SELECT Наименование FROM Финансирование WHERE Финансирование.IDФинансирования = ПланПриема.IDФинансирования) = 'Бюджет' THEN Количество ELSE 0 END AS Всего,
	   CASE WHEN (SELECT Наименование FROM Финансирование WHERE Финансирование.IDФинансирования = ПланПриема.IDФинансирования) = 'Бюджет' THEN КоличествоЦелевыхМест ELSE 0 END AS [в том числе на условиях целевой подготовки],
	   CASE WHEN (SELECT Наименование FROM Финансирование WHERE Финансирование.IDФинансирования = ПланПриема.IDФинансирования) = 'Бюджет' THEN 0 ELSE Количество END [на условиях оплаты],
	   (SELECT COUNT(*) FROM Абитуриент WHERE Абитуриент.IDПланаПриема = ПланПриема.IDПланПриема),
	   (SELECT COUNT(*) FROM Абитуриент WHERE Абитуриент.IDПланаПриема = ПланПриема.IDПланПриема AND Абитуриент.ЦелевойДоговор = 1),
	   (SELECT COUNT(*) FROM Абитуриент WHERE Абитуриент.IDПланаПриема = ПланПриема.IDПланПриема AND (Абитуриент.Сирота = 1 OR (SELECT MIN(Приоритет) FROM СтатьиАбитуриента JOIN Статьи ON (СтатьиАбитуриента.IDСтатьи = Статьи.IDСтатьи) WHERE Абитуриент.IDАбитуриента = IDАбитуриента)= 0))
FROM ПланПриема
WHERE @spec = (SELECT Наименование FROM Специальность WHERE Специальность.IDСпециальность = ПланПриема.IDСпециальности)
END
GO
/****** Object:  StoredProcedure [dbo].[HasStatya]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[HasStatya]
@abiturient int,
@statya nvarchar(50)
AS
BEGIN
SELECT * FROM СтатьиАбитуриента WHERE IDАбитуриента = @abiturient AND IDСтатьи = (SELECT IDСтатьи FROM Статьи WHERE ПолноеНаименование = @statya)
END
GO
/****** Object:  StoredProcedure [dbo].[ImportAD]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[ImportAD]
@id int
as
begin
SELECT Фамилия, Имя, Отчество, 
		(SELECT TOP(1) Сведения FROM КонтактныеДанные WHERE IDТипКонтакта = (SELECT IDТипКонтакта FROM ТипКонтакта WHERE Наименование = 'Домашний телефон' AND КонтактныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента)),
		(SELECT TOP(1) Сведения FROM КонтактныеДанные WHERE IDТипКонтакта = (SELECT IDТипКонтакта FROM ТипКонтакта WHERE Наименование = 'Мобильные телефон' AND КонтактныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента)),
		(SELECT TOP(1) Сведения FROM КонтактныеДанные WHERE IDТипКонтакта = (SELECT IDТипКонтакта FROM ТипКонтакта WHERE Наименование = 'Домашний адрес' AND КонтактныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента)),
		(SELECT ДатаРождения FROM ПаспортныеДанные WHERE ПаспортныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента),
		(SELECT Серия FROM ПаспортныеДанные WHERE ПаспортныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента),
		(SELECT НомерПаспорта FROM ПаспортныеДанные WHERE ПаспортныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента),
		(SELECT ИдентификационныйНомер FROM ПаспортныеДанные WHERE ПаспортныеДанные.IDАбитуриента = Абитуриент.IDАбитуриента)
	FROM Абитуриент
	where IDПланаПриема = @id AND АбитуриентЗачислен = 1
end
GO
/****** Object:  StoredProcedure [dbo].[NextExamList]    Script Date: 10.05.2022 15:47:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[NextExamList]
@id int
AS
BEGIN
SELECT MAX([dbo].[GetNum](ЭкзаменационныйЛист + 'aaa')) + 1
from Абитуриент
where IDПланаПриема = @id
END
GO
/****** Object:  StoredProcedure [dbo].[Update_MainData]    Script Date: 10.05.2022 15:47:54 ******/
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
	UPDATE Абитуриент SET Фамилия = @surename,
						  Имя = @name,
						  Отчество = @otchestvo,
						  Школа = @shool,
						  ГодОкончанияШколы = @graduationYear,
						  ГражданинРБ = @grajdaninRB,
						  Гражданство = @grajdanstvo,
						  Общежитие = @obshejitie, 
						  IDПланаПриема = @planPriema,
						  МестоРаботы = @workPlase,
						  Должность = @doljnost,
						  Сирота = @sirota,
						  ЦелевойДоговор = @dogovor,
						  IDРедактора = @redaktor,
						  ДатаРедактирования = GETDATE(),
						  ЭкзаменационныйЛист = @ExamList
					WHERE IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Update_PasportData]    Script Date: 10.05.2022 15:47:54 ******/
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
	UPDATE ПаспортныеДанные SET 
				ДатаВыдачи = @dateVidachi,
				ДатаРождения = @dateOfBirth,
				Серия = @seriya,
				НомерПаспорта = @pasportNum,
				НаименованиеОргана = @vidan,
				ИдентификационныйНомер = @identNum 
		WHERE IDАбитуриента = @abiturient
END
GO
/****** Object:  StoredProcedure [dbo].[Update_PlanPriema]    Script Date: 10.05.2022 15:47:54 ******/
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
@CT bit,
@Code varchar(13)
AS
BEGIN
UPDATE ПланПриема 
	SET IDСпециальности = (SELECT IDСпециальность FROM Специальность WHERE Наименование = @spec),
		IDФормаОбучения = (SELECT IDФормаОбучения FROM ФормаОбучения WHERE Наименование = @form AND Образование = @obr),
		IDФинансирования = (SELECT IDФинансирования FROM Финансирование WHERE Наименование = @fin),
		Количество = @kolva,
		КоличествоЦелевыхМест = @kolvaCel,
		ЦТ = @CT,
		КодСпециальности = @Code
	WHERE
		IDПланПриема = @id
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
         Begin Table = "Атестат_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 273
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Абитуриент"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 272
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
