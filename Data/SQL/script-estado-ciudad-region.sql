USE [SaludsaAdministracion]
GO
/****** Object:  Table [dbo].[Ciudad]    Script Date: 06/12/2018 3:04:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ciudad](
	[Id] [bigint] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Codigo] [nvarchar](100) NOT NULL,
	[Provincia] [nvarchar](100) NOT NULL,
	[Direccion] [nvarchar](500) NOT NULL,
	[RegionId] [bigint] NULL,
 CONSTRAINT [PK_dbo.Ciudad] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Estado]    Script Date: 06/12/2018 3:04:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estado](
	[Id] [bigint] NOT NULL,
	[Descripcion] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_dbo.Estado] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Region]    Script Date: 06/12/2018 3:04:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Region](
	[Id] [bigint] NOT NULL,
	[Descripcion] [nvarchar](100) NOT NULL,
	[CodigoAuxiliar] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_dbo.Region] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (1, N'Ambato', N'AMB', N'Tungurahua', N'Mera 321 y Rocafuerte, Centro Comercial la Galeria', 2)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (2, N'Cuenca', N'CUE', N'Azuay', N'Av. 12 de Abril y Calle de El Batán, Centro Comercial Los Nogales 07 281', 3)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (3, N'Guayaquil', N'GYE', N'Guayas', N'Av. Carlos julio Arosemena Km. 3 (Junto al Colegio 28 de Mayo)', 1)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (4, N'Ibarra', N'IBA', N'Imbabura', N'Av. Teodoro Gómez de la Torre N° 5-38, Entre Sucre y Rocafuerte', 2)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (5, N'Loja', N'LOJ', N'Loja', N'Bernardo Valdivieso y Quito (esquina)', 3)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (6, N'Machala', N'MAC', N'El Oro', N'Av. Juán Montalvo E/ Arízaga y General Serrano. PB del Teatro Utopia', 1)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (7, N'Manta', N'MAN', N'Manabi', N'Av. Flavio Reyes E/ Calle 28 y 29 Edif. Platinum PB Local I', 1)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (8, N'Quito', N'UIO', N'Pichincha', N'Av. República del Salvador N36-84 y Naciones Unidas', 2)
INSERT [dbo].[Ciudad] ([Id], [Nombre], [Codigo], [Provincia], [Direccion], [RegionId]) VALUES (9, N'Santo Domingo', N'STO', N'Santo Domingo de los Tsáchilas', N'Calle Rio Yamboya s/n y Av. Quito', 2)
INSERT [dbo].[Estado] ([Id], [Descripcion]) VALUES (1, N'ACTIVO')
INSERT [dbo].[Estado] ([Id], [Descripcion]) VALUES (2, N'INACTIVO')
INSERT [dbo].[Region] ([Id], [Descripcion], [CodigoAuxiliar]) VALUES (1, N'Costa', N'00200')
INSERT [dbo].[Region] ([Id], [Descripcion], [CodigoAuxiliar]) VALUES (2, N'Sierra', N'00100')
INSERT [dbo].[Region] ([Id], [Descripcion], [CodigoAuxiliar]) VALUES (3, N'Austro', N'00300')
INSERT [dbo].[Region] ([Id], [Descripcion], [CodigoAuxiliar]) VALUES (4, N'Nacional', N'00900')
ALTER TABLE [dbo].[Ciudad]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Ciudad_dbo.Region_RegionId] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([Id])
GO
ALTER TABLE [dbo].[Ciudad] CHECK CONSTRAINT [FK_dbo.Ciudad_dbo.Region_RegionId]
GO
