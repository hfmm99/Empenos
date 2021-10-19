


ALTER TABLE dbo.Parámetros
	ADD
	   IVA MONEY NULL;

UPDATE dbo.Parámetros
set
      IVA = 0.13

ALTER TABLE dbo.Ventas 
   ADD 
       Impuesto MONEY NULL, 
       Total INT NULL;


UPDATE dbo.Ventas
set
      Impuesto = 0
      ,Total = 0
 WHERE Total is NULL

 GO