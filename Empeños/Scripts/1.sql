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
ALTER TABLE dbo.Empeños
DROP CONSTRAINT FK_Empeños__Clientes
GO
ALTER TABLE dbo.Clientes SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Clientes', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Clientes', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Clientes', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Empeños
(
Código int NOT NULL,
Código_Cliente varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
Fecha datetime NOT NULL,
TotalMontoPréstamo int NOT NULL,
Estado tinyint NOT NULL,
Plazo int NOT NULL,
PorcentajeInterés money NOT NULL,
Notas varchar(500) NULL,
Firma varchar(MAX) NULL
) ON [PRIMARY]
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Empeños SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant
SET @v = N'0 = Activo, 1 = Retirado; 2 = Quedado; 3 = Inactivo'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_Empeños', N'COLUMN', N'Estado'
GO
ALTER TABLE dbo.Tmp_Empeños ADD CONSTRAINT
DF_Empeños_Plazo DEFAULT 3 FOR Plazo
GO
ALTER TABLE dbo.Tmp_Empeños ADD CONSTRAINT
DF_Empeños_PorcentajeInterés DEFAULT 10 FOR PorcentajeInterés
GO
IF EXISTS(SELECT * FROM dbo.Empeños)
EXEC('INSERT INTO dbo.Tmp_Empeños (Código, Código_Cliente, Fecha, TotalMontoPréstamo, Estado, Notas, Firma)
SELECT Código, Código_Cliente, Fecha, TotalMontoPréstamo, Estado, Notas, Firma FROM dbo.Empeños WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.EmpeñosGarantías
DROP CONSTRAINT FK_EmpeñosGarantías__Empeños
GO
ALTER TABLE dbo.EmpeñosDetalle
DROP CONSTRAINT FK_EmpeñosDetalle__Empeños
GO
ALTER TABLE dbo.EmpeñosPagos
DROP CONSTRAINT FK_EmpeñosPagos__Empeños
GO
DROP TABLE dbo.Empeños
GO
EXECUTE sp_rename N'dbo.Tmp_Empeños', N'Empeños', 'OBJECT'
GO
ALTER TABLE dbo.Empeños ADD CONSTRAINT
PK__tmp_ms_x__D68C8CB1504971E0 PRIMARY KEY CLUSTERED
(
Código
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Empeños WITH NOCHECK ADD CONSTRAINT
FK_Empeños__Clientes FOREIGN KEY
(
Código_Cliente
) REFERENCES dbo.Clientes
(
Código
) ON UPDATE NO ACTION
ON DELETE NO ACTION

GO
COMMIT
select Has_Perms_By_Name(N'dbo.Empeños', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Empeños', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Empeños', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.EmpeñosPagos WITH NOCHECK ADD CONSTRAINT
FK_EmpeñosPagos__Empeños FOREIGN KEY
(
Código_Empeño
) REFERENCES dbo.Empeños
(
Código
) ON UPDATE NO ACTION
ON DELETE NO ACTION

GO
ALTER TABLE dbo.EmpeñosPagos SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.EmpeñosPagos', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.EmpeñosPagos', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.EmpeñosPagos', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.EmpeñosDetalle WITH NOCHECK ADD CONSTRAINT
FK_EmpeñosDetalle__Empeños FOREIGN KEY
(
Código_Empeño
) REFERENCES dbo.Empeños
(
Código
) ON UPDATE NO ACTION
ON DELETE NO ACTION

GO
ALTER TABLE dbo.EmpeñosDetalle SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.EmpeñosDetalle', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.EmpeñosDetalle', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.EmpeñosDetalle', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.EmpeñosGarantías WITH NOCHECK ADD CONSTRAINT
FK_EmpeñosGarantías__Empeños FOREIGN KEY
(
Código_Empeño
) REFERENCES dbo.Empeños
(
Código
) ON UPDATE NO ACTION
ON DELETE NO ACTION

GO
ALTER TABLE dbo.EmpeñosGarantías SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.EmpeñosGarantías', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.EmpeñosGarantías', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.EmpeñosGarantías', 'Object', 'CONTROL') as Contr_Per 