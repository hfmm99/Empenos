UPDATE  Ventas
    SET Total = VA.Monto
    FROM Ventas V
    INNER JOIN VentasAbonos VA
        ON V.C�digo = VA.C�digo_Venta 
	WHERE V.Impuesto = 0



