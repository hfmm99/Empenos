﻿using Empeños.Datos;
using System;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Empeños.Reportes
{
    public static class Recibos
    {
        private static readonly double anchoPágina = (double)new LengthConverter().ConvertFrom((object)"7,6cm");
        private static readonly double margenPágina = (double)new LengthConverter().ConvertFrom((object)"0,7cm");
        private const string formatoMoneda = "C0";

        public static FlowDocument ReciboDeCompra(int códigoCompra)
        {
            var flowDocument1 = (FlowDocument)null;

            using (var empeñosDataContext = new EmpeñosDataContext())
            {
                var compra = empeñosDataContext.Compras.SingleOrDefault(c => c.Código == códigoCompra);
                if (compra != null)
                {
                    Image image = new Image()
                    {
                        Stretch = Stretch.None
                    };
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "Recursos\\Imágenes\\Logo.png"), UriKind.Absolute);
                    bitmapImage.EndInit();
                    image.Source = (ImageSource)bitmapImage;
                    FlowDocument flowDocument2 = new FlowDocument();
                    flowDocument2.PageWidth = Recibos.anchoPágina;
                    Thickness thickness = new Thickness(Recibos.margenPágina);
                    flowDocument2.PagePadding = thickness;
                    double num1 = double.NaN;
                    flowDocument2.PageHeight = num1;
                    FontFamily fontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana");
                    flowDocument2.FontFamily = fontFamily;
                    double num2 = 13.0;
                    flowDocument2.FontSize = num2;
                    flowDocument2.Blocks.Add((Block)new BlockUIContainer((UIElement)image));
                    BlockCollection blocks1 = flowDocument2.Blocks;
                    Paragraph paragraph1 = new Paragraph();
                    int num3 = 2;
                    paragraph1.TextAlignment = (TextAlignment)num3;
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Compra Venta & Casa de Empeño"));
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines1 = paragraph1.Inlines;
                    Run run1 = new Run("La Salvada");
                    double num4 = 18.0;
                    run1.FontSize = num4;
                    FontWeight bold1 = FontWeights.Bold;
                    run1.FontWeight = bold1;
                    inlines1.Add(run1);
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Tel: 2474-0641"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Aguas Zarcas - San Carlos"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines2 = paragraph1.Inlines;
                    Run run2 = new Run("FACTURA DE COMPRA NO. " + (object)compra.Código);
                    FontWeight bold2 = FontWeights.Bold;
                    run2.FontWeight = bold2;
                    double num5 = 18.0;
                    run2.FontSize = num5;
                    inlines2.Add(run2);
                    blocks1.Add((Block)paragraph1);
                    BlockCollection blocks2 = flowDocument2.Blocks;
                    Paragraph paragraph2 = new Paragraph();
                    int num6 = 0;
                    paragraph2.TextAlignment = (TextAlignment)num6;
                    paragraph2.Inlines.Add(new Run("Fecha  : " + compra.Fecha.ToString("dd/MMM/yyyy")));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new Run("Cliente: " + compra.Cliente.NombreCompleto));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new Run("Cédula : " + compra.Código_Cliente));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new LineBreak());
                    InlineCollection inlines3 = paragraph2.Inlines;
                    Line line = new Line();
                    int num7 = 1;
                    line.Stretch = (Stretch)num7;
                    SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
                    line.Stroke = (Brush)solidColorBrush;
                    double num8 = 1.0;
                    line.X2 = num8;
                    inlines3.Add(line);
                    blocks2.Add(paragraph2);
                    flowDocument1 = flowDocument2;
                    foreach (ComprasDetalle compraDetalles in compra.ComprasDetalles)
                    {
                        BlockCollection blocks3 = flowDocument1.Blocks;
                        Paragraph paragraph3 = new Paragraph();
                        int num9 = 0;
                        paragraph3.TextAlignment = (TextAlignment)num9;
                        paragraph3.Inlines.Add(new Run(compraDetalles.Artículo.ToString()));
                        blocks3.Add((Block)paragraph3);
                    }
                    BlockCollection blocks4 = flowDocument1.Blocks;
                    Block[] blockArray = new Block[4];
                    int index1 = 0;
                    Paragraph paragraph4 = new Paragraph();
                    int num10 = 1;
                    paragraph4.TextAlignment = (TextAlignment)num10;
                    paragraph4.Inlines.Add(new Run("Monto Total: "));
                    paragraph4.Inlines.Add((compra.ComprasDetalles.Sum(det => det.Artículo.Costo)).ToString("C0"));
                    blockArray[index1] = (Block)paragraph4;
                    int index2 = 1;
                    Paragraph paragraph5 = new Paragraph();
                    int num11 = 3;
                    paragraph5.TextAlignment = (TextAlignment)num11;
                    double num12 = 10.0;
                    paragraph5.FontSize = num12;
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    blockArray[index2] = (Block)paragraph5;
                    int index3 = 2;
                    Paragraph paragraph6 = new Paragraph();
                    int num13 = 2;
                    paragraph6.TextAlignment = (TextAlignment)num13;
                    paragraph6.Inlines.Add(new Run("GRACIAS POR PREFERIRNOS."));
                    blockArray[index3] = (Block)paragraph6;
                    int index4 = 3;
                    Paragraph paragraph7 = new Paragraph();
                    int num14 = 2;
                    paragraph7.TextAlignment = (TextAlignment)num14;
                    InlineCollection inlines4 = paragraph7.Inlines;
                    paragraph7.Inlines.Add(new LineBreak());
                    paragraph7.Inlines.Add(new LineBreak());
                    blockArray[index4] = (Block)paragraph7;
                    blocks4.AddRange((IEnumerable)blockArray);
                }
            }

            return flowDocument1;
        }


        public static FlowDocument ReciboDeEmpeño(int códigoEmpeño)
        {
            FlowDocument flowDocument1 = (FlowDocument)null;
            EmpeñosDataContext empeñosDataContext = new EmpeñosDataContext();
            try
            {
                Empeño empeño = empeñosDataContext.Empeños.SingleOrDefault<Empeño>((System.Linq.Expressions.Expression<Func<Empeño, bool>>)(emp => emp.Código == códigoEmpeño));
                if (empeño != null)
                {
                    Image image = new Image()
                    {
                        Stretch = Stretch.None
                    };
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "Recursos\\Imágenes\\Logo.png"), UriKind.Absolute);
                    bitmapImage.EndInit();
                    image.Source = (ImageSource)bitmapImage;
                    FlowDocument flowDocument2 = new FlowDocument();
                    flowDocument2.PageWidth = Recibos.anchoPágina;
                    Thickness thickness = new Thickness(Recibos.margenPágina);
                    flowDocument2.PagePadding = thickness;
                    double num1 = double.NaN;
                    flowDocument2.PageHeight = num1;
                    FontFamily fontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana");
                    flowDocument2.FontFamily = fontFamily;
                    double num2 = 13.0;
                    flowDocument2.FontSize = num2;
                    flowDocument2.Blocks.Add((Block)new BlockUIContainer((UIElement)image));
                    BlockCollection blocks1 = flowDocument2.Blocks;
                    Paragraph paragraph1 = new Paragraph();
                    int num3 = 2;
                    paragraph1.TextAlignment = (TextAlignment)num3;
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Compra Venta & Casa de Empeño"));
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines1 = paragraph1.Inlines;
                    Run run1 = new Run("La Salvada");
                    double num4 = 18.0;
                    run1.FontSize = num4;
                    FontWeight bold1 = FontWeights.Bold;
                    run1.FontWeight = bold1;
                    inlines1.Add(run1);
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Tel: 2474-0641"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Aguas Zarcas - San Carlos"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines2 = paragraph1.Inlines;
                    Run run2 = new Run("EMPEÑO NO. " + (object)empeño.Código);
                    FontWeight bold2 = FontWeights.Bold;
                    run2.FontWeight = bold2;
                    double num5 = 18.0;
                    run2.FontSize = num5;
                    inlines2.Add(run2);
                    blocks1.Add((Block)paragraph1);
                    BlockCollection blocks2 = flowDocument2.Blocks;
                    Paragraph paragraph2 = new Paragraph();
                    int num6 = 0;
                    paragraph2.TextAlignment = (TextAlignment)num6;
                    paragraph2.Inlines.Add(new Run("Fecha  : " + empeño.Fecha.ToString("dd/MMM/yyyy")));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new Run("Cliente: " + empeño.Cliente.NombreCompleto));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new Run("Cédula : " + empeño.Código_Cliente));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new LineBreak());
                    InlineCollection inlines3 = paragraph2.Inlines;
                    Line line = new Line();
                    int num7 = 1;
                    line.Stretch = (Stretch)num7;
                    SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
                    line.Stroke = (Brush)solidColorBrush;
                    double num8 = 1.0;
                    line.X2 = num8;
                    inlines3.Add((UIElement)line);
                    blocks2.Add((Block)paragraph2);
                    flowDocument1 = flowDocument2;
                    foreach (EmpeñosDetalle empeñosDetalle in empeño.EmpeñosDetalles)
                    {
                        BlockCollection blocks3 = flowDocument1.Blocks;
                        Paragraph paragraph3 = new Paragraph();
                        int num9 = 0;
                        paragraph3.TextAlignment = (TextAlignment)num9;
                        paragraph3.Inlines.Add(new Run(empeñosDetalle.Artículo.ToString()));
                        blocks3.Add((Block)paragraph3);
                    }
                    BlockCollection blocks4 = flowDocument1.Blocks;
                    Block[] blockArray = new Block[2];
                    int index1 = 0;
                    Paragraph paragraph4 = new Paragraph();
                    int num10 = 1;
                    paragraph4.TextAlignment = (TextAlignment)num10;
                    paragraph4.Inlines.Add(new Run("Total Préstamo: "));
                    paragraph4.Inlines.Add(empeño.TotalMontoPréstamo.ToString("C0"));
                    blockArray[index1] = (Block)paragraph4;
                    int index2 = 1;
                    Paragraph paragraph5 = new Paragraph();
                    int num11 = 3;
                    paragraph5.TextAlignment = (TextAlignment)num11;
                    double num12 = 10.0;
                    paragraph5.FontSize = num12;
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());

                    paragraph5.Inlines.Add(new Run { Text = "Firma:_____________________________", FontSize = 12 });
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new Run { Text = "Cédula:____________________________", FontSize = 12 });

                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new Run("1. Todo préstamo, es hecho a un plazo de trés meses, por un mes de intereses cancelado renueva el plazo de vencimiento un més."));
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new Run("2. Tasa de interés del 10% mensual, cobrandose siempre un més como mínimo. Pagaderos por mes."));
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new Run("3. El no pago de intereses en 3 meses autoriza a la Salvada para que disponga de la prenda."));
                    blockArray[index2] = (Block)paragraph5;
                    blocks4.AddRange((IEnumerable)blockArray);
                }
            }
            finally
            {
                if (empeñosDataContext != null)
                    empeñosDataContext.Dispose();
            }
            return flowDocument1;
        }

        public static FlowDocument ReciboDePago(string códigoEmpeño, string códigoCliente, string nombreCliente, int cuota, DateTime fecha, DateTime fechaPago, int intereses, int abono, int saldo, string firma)
        {
            Image image = new Image()
            {
                Stretch = Stretch.None
            };
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "Recursos\\Imágenes\\Logo.png"), UriKind.Absolute);
            bitmapImage.EndInit();
            image.Source = (ImageSource)bitmapImage;

            FlowDocument flowDocument1 = new FlowDocument();
            flowDocument1.PageWidth = Recibos.anchoPágina;
            Thickness thickness = new Thickness(Recibos.margenPágina);
            flowDocument1.PagePadding = thickness;
            double num1 = double.NaN;
            flowDocument1.PageHeight = num1;
            FontFamily fontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana");
            flowDocument1.FontFamily = fontFamily;
            double num2 = 13.0;
            flowDocument1.FontSize = num2;
            flowDocument1.Blocks.Add((Block)new BlockUIContainer((UIElement)image));
            BlockCollection blocks1 = flowDocument1.Blocks;
            Paragraph paragraph1 = new Paragraph();
            int num3 = 2;
            paragraph1.TextAlignment = (TextAlignment)num3;
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new Run("Compra Venta & Casa de Empeño"));
            paragraph1.Inlines.Add(new LineBreak());
            InlineCollection inlines1 = paragraph1.Inlines;
            Run run1 = new Run("La Salvada");
            double num4 = 18.0;
            run1.FontSize = num4;
            FontWeight bold1 = FontWeights.Bold;
            run1.FontWeight = bold1;
            inlines1.Add(run1);
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new Run("Tel: 2474-0641"));
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new Run("Aguas Zarcas - San Carlos"));
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new LineBreak());
            InlineCollection inlines2 = paragraph1.Inlines;
            Run run2 = new Run((saldo == 0 ? "RETIRO" : "RENOVACIÓN") + " NO. " + códigoEmpeño);
            FontWeight bold2 = FontWeights.Bold;
            run2.FontWeight = bold2;
            double num5 = 18.0;
            run2.FontSize = num5;
            inlines2.Add(run2);
            blocks1.Add((Block)paragraph1);
            BlockCollection blocks2 = flowDocument1.Blocks;
            Paragraph paragraph2 = new Paragraph();
            int num6 = 0;
            paragraph2.TextAlignment = (TextAlignment)num6;
            paragraph2.Inlines.Add(new Run("Cliente: " + nombreCliente));
            paragraph2.Inlines.Add(new LineBreak());
            paragraph2.Inlines.Add(new Run("Cédula : " + códigoCliente));
            paragraph2.Inlines.Add(new LineBreak());
            paragraph2.Inlines.Add(new Run("Paga Desde: " + fecha.AddMonths(-1).ToString("dd/MMM/yyyy")));
            paragraph2.Inlines.Add(new LineBreak());
            paragraph2.Inlines.Add(new Run("Paga Hasta: " + fecha.ToString("dd/MMM/yyyy")));
            paragraph2.Inlines.Add(new LineBreak());
            paragraph2.Inlines.Add(new Run("Fecha de Pago: " + fechaPago.ToString("dd/MMM/yyyy hh:mm:ss tt")));
            paragraph2.Inlines.Add(new LineBreak());
            paragraph2.Inlines.Add(new LineBreak());
            InlineCollection inlines3 = paragraph2.Inlines;
            Line line = new Line();
            int num7 = 1;
            line.Stretch = (Stretch)num7;
            SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
            line.Stroke = (Brush)solidColorBrush;
            double num8 = 1.0;
            line.X2 = num8;
            inlines3.Add((UIElement)line);
            paragraph2.Inlines.Add(new LineBreak());
            blocks2.Add((Block)paragraph2);
            BlockCollection blocks3 = flowDocument1.Blocks;
            System.Windows.Documents.Table table = new System.Windows.Documents.Table();
            TableRowGroupCollection rowGroups = table.RowGroups;
            TableRowGroup tableRowGroup = new TableRowGroup();
            TableRowCollection rows1 = tableRowGroup.Rows;
            TableRowCollection rows2 = tableRowGroup.Rows;
            TableRow tableRow2 = new TableRow();
            TableCellCollection cells3 = tableRow2.Cells;
            Paragraph paragraph5 = new Paragraph();
            paragraph5.Inlines.Add(new Run("Intereses : "));
            cells3.Add(new TableCell((Block)paragraph5)
            {
                TextAlignment = TextAlignment.Left
            });
            TableCellCollection cells4 = tableRow2.Cells;
            Paragraph paragraph6 = new Paragraph();
            paragraph6.Inlines.Add(new Run(intereses.ToString("C0")));
            cells4.Add(new TableCell((Block)paragraph6)
            {
                TextAlignment = TextAlignment.Right
            });
            rows2.Add(tableRow2);
            TableRowCollection rows3 = tableRowGroup.Rows;
            TableRow tableRow3 = new TableRow();
            TableCellCollection cells5 = tableRow3.Cells;
            Paragraph paragraph7 = new Paragraph();
            paragraph7.Inlines.Add(new Run("Abono : "));
            cells5.Add(new TableCell((Block)paragraph7)
            {
                TextAlignment = TextAlignment.Left
            });
            TableCellCollection cells6 = tableRow3.Cells;
            Paragraph paragraph8 = new Paragraph();
            paragraph8.Inlines.Add(new Run(abono.ToString("C0")));
            cells6.Add(new TableCell((Block)paragraph8)
            {
                TextAlignment = TextAlignment.Right
            });
            rows3.Add(tableRow3);
            tableRowGroup.Rows.Add(new TableRow());
            TableRowCollection rows4 = tableRowGroup.Rows;
            TableRow tableRow4 = new TableRow();
            TableCellCollection cells7 = tableRow4.Cells;
            Paragraph paragraph9 = new Paragraph();
            paragraph9.Inlines.Add(new Run("Saldo : "));
            cells7.Add(new TableCell((Block)paragraph9)
            {
                TextAlignment = TextAlignment.Left
            });
            TableCellCollection cells8 = tableRow4.Cells;
            Paragraph paragraph10 = new Paragraph();
            paragraph10.Inlines.Add(new Run(saldo.ToString("C0")));
            cells8.Add(new TableCell((Block)paragraph10)
            {
                TextAlignment = TextAlignment.Right
            });
            rows4.Add(tableRow4);
            rowGroups.Add(tableRowGroup);
            blocks3.Add((Block)table);
            FlowDocument flowDocument2 = flowDocument1;

            BlockCollection blocks4 = flowDocument2.Blocks;
            Paragraph paragraph11 = new Paragraph();
            int num9 = 3;
            paragraph11.TextAlignment = (TextAlignment)num9;
            double num10 = 10.0;
            paragraph11.FontSize = num10;

            if (firma != null && firma.Length > 0)
            {
                paragraph11.Inlines.Add(new Run { Text = "Firma:", FontSize = 12 });

                var inkFirma = new Topaz.SigPlusNET();

                // Encriptar y Comprimir Firma
                inkFirma.SetSigCompressionMode(2);
                inkFirma.SetEncryptionMode(2);
                inkFirma.AutoKeyStart();
                inkFirma.SetAutoKeyData("La Salvada");
                inkFirma.AutoKeyFinish();

                inkFirma.SetImageXSize(500);
                inkFirma.SetImageYSize(150);
                inkFirma.SetJustifyMode(5);

                inkFirma.SetSigString(firma);

                var img = new BitmapImage();
                var ms = new MemoryStream();
                inkFirma.GetSigImage().Save(ms, ImageFormat.Bmp);

                var imgFirma = new BitmapImage();
                imgFirma.BeginInit();
                imgFirma.StreamSource = ms;
                imgFirma.EndInit();

                paragraph11.Inlines.Add(new Image { Source = imgFirma });
            }
            else
            {
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new LineBreak());

                paragraph11.Inlines.Add(new Run { Text = "Firma:_____________________________", FontSize = 12 });

            }
            paragraph11.Inlines.Add(new LineBreak());
            paragraph11.Inlines.Add(new LineBreak());
            paragraph11.Inlines.Add(new LineBreak());
            paragraph11.Inlines.Add(new Run { Text = "Cédula:____________________________", FontSize = 12 });

            paragraph11.Inlines.Add(new LineBreak());
            paragraph11.Inlines.Add(new LineBreak());

            if (saldo > 0)
            {
                paragraph11.Inlines.Add(new Run("1. Todo préstamo, es hecho a un plazo de trés meses, por un mes de intereses cancelado renueva el plazo de vencimiento un més."));
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new Run("2. Tasa de interés del 10% mensual, cobrandose siempre un més como mínimo. Pagaderos por mes."));
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new Run("3. El no pago de intereses en 3 meses autoriza a la Salvada para que disponga de la prenda."));
                blocks4.Add((Block)paragraph11);
            }
            else
            {
                paragraph11.Inlines.Add(new Run("No se aceptarán reclamos sobre artículos retirados, una vez que salgan de nuestras instalaciones."));
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new LineBreak());
                InlineCollection inlines5 = paragraph11.Inlines;
                blocks4.Add((Block)paragraph11);
            }
            return flowDocument2;
        }

        public static FlowDocument ReciboDeVenta(int códigoVenta)
        {
            FlowDocument flowDocument1 = (FlowDocument)null;
            using (EmpeñosDataContext empeñosDataContext = new EmpeñosDataContext())
            {
                Venta venta = empeñosDataContext.Ventas.SingleOrDefault<Venta>((System.Linq.Expressions.Expression<Func<Venta, bool>>)(ven => ven.Código == códigoVenta));
                if (venta != null)
                {
                    Image image = new Image()
                    {
                        Stretch = Stretch.None
                    };
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "Recursos\\Imágenes\\Logo.png"), UriKind.Absolute);
                    bitmapImage.EndInit();
                    image.Source = (ImageSource)bitmapImage;
                    FlowDocument flowDocument2 = new FlowDocument();
                    flowDocument2.PageWidth = Recibos.anchoPágina;
                    Thickness thickness = new Thickness(Recibos.margenPágina);
                    flowDocument2.PagePadding = thickness;
                    double num1 = double.NaN;
                    flowDocument2.PageHeight = num1;
                    FontFamily fontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana");
                    flowDocument2.FontFamily = fontFamily;
                    double num2 = 13.0;
                    flowDocument2.FontSize = num2;
                    flowDocument2.Blocks.Add((Block)new BlockUIContainer((UIElement)image));
                    BlockCollection blocks1 = flowDocument2.Blocks;
                    Paragraph paragraph1 = new Paragraph();
                    int num3 = 2;
                    paragraph1.TextAlignment = (TextAlignment)num3;
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Compra Venta & Casa de Empeño"));
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines1 = paragraph1.Inlines;
                    Run run1 = new Run("La Salvada");
                    double num4 = 18.0;
                    run1.FontSize = num4;
                    FontWeight bold1 = FontWeights.Bold;
                    run1.FontWeight = bold1;
                    inlines1.Add(run1);
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Tel: 2474-0641"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Aguas Zarcas - San Carlos"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines2 = paragraph1.Inlines;
                    Run run2 = new Run("FACTURA DE VENTA NO. " + (object)venta.Código);
                    FontWeight bold2 = FontWeights.Bold;
                    run2.FontWeight = bold2;
                    double num5 = 18.0;
                    run2.FontSize = num5;
                    inlines2.Add(run2);
                    blocks1.Add((Block)paragraph1);
                    BlockCollection blocks2 = flowDocument2.Blocks;
                    Paragraph paragraph2 = new Paragraph();
                    int num6 = 0;
                    paragraph2.TextAlignment = (TextAlignment)num6;
                    paragraph2.Inlines.Add(new Run("Fecha  : " + venta.Fecha.ToString("dd/MMM/yyyy")));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new Run("Cliente: " + venta.Cliente.NombreCompleto));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new Run("Cédula : " + venta.Código_Cliente));
                    paragraph2.Inlines.Add(new LineBreak());
                    paragraph2.Inlines.Add(new LineBreak());
                    InlineCollection inlines3 = paragraph2.Inlines;
                    Line line = new Line();
                    int num7 = 1;
                    line.Stretch = (Stretch)num7;
                    SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
                    line.Stroke = (Brush)solidColorBrush;
                    double num8 = 1.0;
                    line.X2 = num8;
                    inlines3.Add((UIElement)line);
                    blocks2.Add((Block)paragraph2);
                    flowDocument1 = flowDocument2;
                    foreach (VentasDetalle ventasDetalle in venta.VentasDetalles)
                    {
                        BlockCollection blocks3 = flowDocument1.Blocks;
                        Paragraph paragraph3 = new Paragraph();
                        int num9 = 0;
                        paragraph3.TextAlignment = (TextAlignment)num9;
                        paragraph3.Inlines.Add(new Run(ventasDetalle.Artículo.ToString()));
                        blocks3.Add((Block)paragraph3);
                    }
                    BlockCollection blocks4 = flowDocument1.Blocks;
                    Block[] blockArray = new Block[4];
                    int index1 = 0;
                    Paragraph paragraph4 = new Paragraph();
                    int num10 = 1;
                    paragraph4.TextAlignment = (TextAlignment)num10;
                    paragraph4.Inlines.Add(new Run("Monto Total: "));
                    paragraph4.Inlines.Add(venta.VentasDetalles.Sum<VentasDetalle>((Func<VentasDetalle, int>)(det => det.Artículo.Precio.Value)).ToString("C0"));
                    blockArray[index1] = (Block)paragraph4;
                    int index2 = 1;
                    Paragraph paragraph5 = new Paragraph();
                    int num11 = 3;
                    paragraph5.TextAlignment = (TextAlignment)num11;
                    double num12 = 10.0;
                    paragraph5.FontSize = num12;
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new LineBreak());
                    paragraph5.Inlines.Add(new Run("1. Favor revisar los artículos antes de salir del local, no se aceptan devoluciones."));
                    blockArray[index2] = (Block)paragraph5;
                    int index3 = 2;
                    Paragraph paragraph6 = new Paragraph();
                    int num13 = 2;
                    paragraph6.TextAlignment = (TextAlignment)num13;
                    paragraph6.Inlines.Add(new Run("GRACIAS POR PREFERIRNOS."));
                    blockArray[index3] = (Block)paragraph6;
                    int index4 = 3;
                    Paragraph paragraph7 = new Paragraph();
                    int num14 = 2;
                    paragraph7.TextAlignment = (TextAlignment)num14;
                    paragraph7.Inlines.Add(new LineBreak());
                    paragraph7.Inlines.Add(new LineBreak());
                    blockArray[index4] = (Block)paragraph7;
                    blocks4.AddRange((IEnumerable)blockArray);
                }
            }
            return flowDocument1;
        }

        public static void Imprimir(string título, FlowDocument recibo, bool previsualizar = false)
        {
            if (recibo == null)
                return;
            PrintDialog printDialog = new PrintDialog();
            int num;
            if (previsualizar)
            {
                bool? nullable = printDialog.ShowDialog();
                bool flag = true;
                num = nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0;
            }
            else
                num = 1;
            if (num != 0)
                printDialog.PrintDocument(((IDocumentPaginatorSource)recibo).DocumentPaginator, título);
        }
    }
}
