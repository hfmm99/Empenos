UPDATE  Ventas
    SET Total = VA.Monto
    FROM Ventas V
    INNER JOIN VentasAbonos VA
        ON V.Código = VA.Código_Venta 
	WHERE V.Impuesto = 0



