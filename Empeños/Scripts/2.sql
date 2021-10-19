/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Clientes
	DROP CONSTRAINT DF__tmp_ms_xx__Géner__339FAB6E
GO
CREATE TABLE dbo.Tmp_Clientes
	(
	Código varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
	TipoIdenticación smallint NOT NULL,
	Nombre varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
	Apellidos varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
	NombreCompleto  AS (([Nombre]+' ')+[Apellidos]),
	Género char(1) NOT NULL,
	Teléfono varchar(100) NULL,
	Email varchar(100) NOT NULL,
	RecibirNotificaciones bit NOT NULL,
	Dirección varchar(250) NOT NULL,
	Notas varchar(500) NULL,
	Foto varbinary(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Clientes SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Clientes ADD CONSTRAINT
	DF_Clientes_TipoIdenticación DEFAULT 5 FOR TipoIdenticación
GO
ALTER TABLE dbo.Tmp_Clientes ADD CONSTRAINT
	DF__tmp_ms_xx__Géner__339FAB6E DEFAULT ('M') FOR Género
GO
IF EXISTS(SELECT * FROM dbo.Clientes)
	 EXEC('INSERT INTO dbo.Tmp_Clientes (Código, Nombre, Apellidos, Género, Teléfono, Email, RecibirNotificaciones, Dirección, Notas, Foto)
		SELECT Código, Nombre, Apellidos, Género, Teléfono, Email, RecibirNotificaciones, Dirección, Notas, Foto FROM dbo.Clientes WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.Empeños
	DROP CONSTRAINT FK_Empeños__Clientes
GO
ALTER TABLE dbo.Ventas
	DROP CONSTRAINT FK_Ventas__Clientes
GO
ALTER TABLE dbo.Compras
	DROP CONSTRAINT FK_Compras__Clientes
GO
DROP TABLE dbo.Clientes
GO
EXECUTE sp_rename N'dbo.Tmp_Clientes', N'Clientes', 'OBJECT' 
GO
ALTER TABLE dbo.Clientes ADD CONSTRAINT
	PK__tmp_ms_x__D68C8CB129FB682C PRIMARY KEY CLUSTERED 
	(
	Código
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Clientes', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Clientes', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Clientes', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Compras WITH NOCHECK ADD CONSTRAINT
	FK_Compras__Clientes FOREIGN KEY
	(
	Código_Cliente
	) REFERENCES dbo.Clientes
	(
	Código
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Compras SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Compras', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Compras', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Compras', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Ventas WITH NOCHECK ADD CONSTRAINT
	FK_Ventas__Clientes FOREIGN KEY
	(
	Código_Cliente
	) REFERENCES dbo.Clientes
	(
	Código
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Ventas SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Ventas', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Ventas', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Ventas', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Empeños WITH NOCHECK ADD CONSTRAINT
	FK_Empeños__Clientes FOREIGN KEY
	(
	Código_Cliente
	) REFERENCES dbo.Clientes
	(
	Código
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Empeños SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Empeños', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Empeños', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Empeños', 'Object', 'CONTROL') as Contr_Per 