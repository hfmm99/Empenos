using Empeños.Datos;
using Empeños.Formularios;
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
        private static readonly double margenPágina = (double)new LengthConverter().ConvertFrom((object)"0,5cm");
        private const string formatoMoneda = "C0";
        private static readonly double anchoPáginaCarta = (double)new LengthConverter().ConvertFrom((object)"21,6cm");
        private static readonly double largoPáginaCarta = (double)new LengthConverter().ConvertFrom((object)"27,94cm");

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
                    paragraph1.Inlines.Add(new Run("Compra Venta"));
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

        public static FlowDocument ReciboDeEmpeño(int códigoEmpeño, string firma)
        {
            using (var bd = new EmpeñosDataContext())
            {
                var empeño = bd.Empeños.SingleOrDefault<Empeño>(emp => emp.Código == códigoEmpeño);
                var parámetros = bd.Parámetros.SingleOrDefault();

                if (empeño != null && parámetros != null)
                {
                    //Documento con Encabezado
                    var recibo = new FlowDocument
                    {
                        TextAlignment = TextAlignment.Justify,
                        PageWidth = Recibos.anchoPágina,
                        PageHeight = double.NaN,
                        PagePadding = new Thickness(Recibos.margenPágina),
                        FontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana"),
                        FontSize = 13,
                        Blocks =
                        {
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Center,
                                Inlines =
                                {
                                    new Image
                                    {
                                        Stretch = Stretch.None,
                                        Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Recursos\\Imágenes\\Logo.png")))
                                    },
                                    new LineBreak(),
                                    new LineBreak(),
                                    "Compra Venta",
                                    new LineBreak(),
                                    new Run("La Salvada") { FontSize = 18, FontWeight = FontWeights.Bold },
                                    new LineBreak(),
                                    "Porfirio Morales Mora",
                                    new LineBreak(),
                                    "Cédula: 2-0290-0936",
                                    new LineBreak(),
                                    new LineBreak(),
                                    "Tel: 2474-0641",
                                    new LineBreak(),
                                    "Aguas Zarcas - San Carlos",
                                    new LineBreak(),
                                    new LineBreak(),
                                    new Run("COMPRA NO. " + empeño.Código) { FontSize = 18, FontWeight = FontWeights.Bold },
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Left,
                                Inlines =
                                {
                                    $"Fecha  : {empeño.Fecha.ToString("dd/MMM/yyyy")}",
                                    new LineBreak(),
                                    $"Cliente: {empeño.Cliente.NombreCompleto}",
                                    new LineBreak(),
                                    $"Cédula : {empeño.Código_Cliente}",
                                    new LineBreak(),
                                    new LineBreak(),
                                    new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 }
                                }
                            }
                        }
                    };

                    //Artículos
                    foreach (EmpeñosDetalle empeñosDetalle in empeño.EmpeñosDetalles)
                    {
                        recibo.Blocks.Add(new Paragraph
                        {
                            TextAlignment = TextAlignment.Left,
                            Inlines =
                            {
                                empeñosDetalle.Artículo.ToString()
                            }
                        });
                    }

                    // Totales
                    recibo.Blocks.Add
                    (
                        new Paragraph
                        {
                            TextAlignment = TextAlignment.Right,
                            Inlines =
                            {
                                $"Total: {empeño.TotalMontoPréstamo.ToString("C0")}"
                            }
                        }
                    );

                    //Acuerdo de Compra y Venta
                    recibo.Blocks.Add
                    (
                        new Paragraph
                        {
                            FontSize = 10,
                            Inlines =
                            {
                                new LineBreak(),
                                new Run("  ACUERDO DE COMPRA Y VENTA  ") { FontWeight = FontWeights.Bold, FontSize = 14},
                                new LineBreak(),
                                new LineBreak(),
                                "Nosotros, Porfirio Morales Mora,  cédula: 2-0290-0936, de ahora en adelante EL COMPRADOR y ",
                                $"{empeño.Cliente.Nombre} {empeño.Cliente.Apellidos}, cédula: {empeño.Cliente.Código}",
                                ", de ahora en adelante EL VENDEDOR, según se identifican en este contrato.",
                                new LineBreak(),
                                "EL VENDEDOR es dueño de o los artículos que se supra detalla, quien da en venta a EL COMPRADOR y lo libera de toda responsabilidad",
                                new LineBreak(),
                                "EL COMPRADOR lo o los acepta comprar y almacenarlos por 30 dias desde hoy; por si ",
                                "EL VENDEDOR en cualquier momento lo o los desea adquirir, por el periodo de almacenamiento, por el total, mas el 10% del mismo ",
                                "por concepto de almacenamiento de o los artículos. El periodo de almacenamiento podrá extenderse en acuerdo de ambos, cada 30 días ",
                                "realizando el pago del 10% del precio del total cada 30 días, de lo contrario EL COMPRADOR no estará más obligado a mantener el o los articulos almacenados y disponer de este o estos.",
                            }
                        }
                    );

                    recibo.Blocks.Add(
                        new Paragraph
                        {
                            FontSize = 12,
                            TextAlignment = TextAlignment.Center,
                            Inlines =
                            {
                                new Image
                                {
                                    Stretch = Stretch.None,
                                    Source = new BitmapImage(new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Recursos\\Imágenes\\FirmaPorfirio.png")))
                                },
                                new LineBreak(),
                                new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 },
                                new LineBreak(),
                                "Firma COMPRADOR",
                                new LineBreak(),
                                new LineBreak(),
                                "2-0290-0936",
                                new LineBreak(),
                                new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 },
                                new LineBreak(),
                                "Cédula COMPRADOR",
                                new LineBreak()
                            }
                        });

                    if (firma != null && firma.Length > 0)
                    {
                        #region Cargar Firma

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

                        #endregion

                        recibo.Blocks.Add
                        (
                            new Paragraph
                            {
                                FontSize = 12,
                                TextAlignment = TextAlignment.Center,
                                Inlines =
                                {
                                    new Image
                                    {
                                        Source = imgFirma
                                    },
                                    new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 },
                                    "Firma VENDEDOR",
                                    new LineBreak(),
                                    new LineBreak(),
                                    new LineBreak(),
                                    new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 },
                                    "Cédula VENDEDOR"
                                }
                            }
                        );
                    }
                    else
                    {
                        recibo.Blocks.Add
                        (
                            new Paragraph
                            {
                                FontSize = 12,
                                TextAlignment = TextAlignment.Center,
                                Inlines =
                                {
                                    new LineBreak(),
                                    new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 },
                                    new LineBreak(),
                                    "Firma VENDEDOR",
                                    new LineBreak(),
                                    new LineBreak(),
                                    new LineBreak(),
                                    new Line { Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1 },
                                    new LineBreak(),
                                    "Cédula VENDEDOR"
                                }
                            }
                        );
                    }

                    return recibo;
                }

                return new FlowDocument();
            }
        }

        public static FlowDocument ReciboDePago(string códigoEmpeño, string códigoCliente, string nombreCliente, int cuota, DateTime fecha, DateTime fechaPago, int intereses, int abono, int saldo, string firma)
        {
            EmpeñosDataContext empeñosDataContext = new EmpeñosDataContext();
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
            paragraph1.Inlines.Add(new Run("Compra Venta"));
            paragraph1.Inlines.Add(new LineBreak());
            InlineCollection inlines1 = paragraph1.Inlines;
            Run run1 = new Run("La Salvada");
            double num4 = 18.0;
            run1.FontSize = num4;
            FontWeight bold1 = FontWeights.Bold;
            run1.FontWeight = bold1;
            inlines1.Add(run1);
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new Run("Porfirio Morales Mora"));
            paragraph1.Inlines.Add(new LineBreak());
            paragraph1.Inlines.Add(new Run("Cédula: 2-0290-0936"));
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
            // line.Stretch = (Stretch)num7;
            // SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
            // line.Stroke = (Brush)solidColorBrush;
            double num8 = 1.0;
            line.X2 = num8;
            inlines3.Add((UIElement)line);
            paragraph2.Inlines.Add(new LineBreak());
            blocks2.Add((Block)paragraph2);
            /*BlockCollection blocks3 = flowDocument1.Blocks;
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
            blocks3.Add((Block)table);*/
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

                /* Empeño empeño = empeñosDataContext.Empeños.SingleOrDefault<Empeño>((System.Linq.Expressions.Expression<Func<Empeño, bool>>)(emp => emp.Código == int.Parse(códigoEmpeño)));
                 paragraph11.Inlines.Add(new LineBreak());
                 paragraph11.Inlines.Add(new LineBreak());
                 paragraph11.Inlines.Add(new Run($"1. Todo préstamo, es hecho a un plazo de { empeño.Plazo } {  (empeño.Plazo == 1 ? "mes" : "meses") }, por cada mes de intereses cancelado renueva el plazo de vencimiento un mes más."));
                 paragraph11.Inlines.Add(new LineBreak());
                 string value = Math.Round(empeño.PorcentajeInterés, 2).ToString();
                 paragraph11.Inlines.Add(new Run("2. Tasa de interés del " + value + "% mensual, cobrandose siempre un mes como mínimo. Pagaderos mensualmente."));

                 paragraph11.Inlines.Add(new LineBreak());
                 paragraph11.Inlines.Add(new Run($"3. El no pago de intereses en { empeño.Plazo }{(empeño.Plazo == 1 ? " mes" : " meses") } autoriza a La Salvada a disponer de la prenda empeñada."));
                 paragraph11.Inlines.Add(new LineBreak());
                 paragraph11.Inlines.Add(new LineBreak());
                 paragraph11.Inlines.Add(new Run("No olvide su próximo pago el: " + fecha.AddMonths(1).ToShortDateString()));*/
                blocks4.Add((Block)paragraph11);
            }
            else
            {
                paragraph11.Inlines.Add(new Run("No se aceptarán reclamos sobre artículos retirados, una vez que salgan de nuestras instalaciones."));
                paragraph11.Inlines.Add(new LineBreak());
                paragraph11.Inlines.Add(new LineBreak());
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
                    paragraph1.Inlines.Add(new Run("Compra Venta"));
                    paragraph1.Inlines.Add(new LineBreak());
                    InlineCollection inlines1 = paragraph1.Inlines;
                    Run run1 = new Run("La Salvada");
                    double num4 = 18.0;
                    run1.FontSize = num4;
                    FontWeight bold1 = FontWeights.Bold;
                    run1.FontWeight = bold1;
                    inlines1.Add(run1);
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Porfirio Morales Mora"));
                    paragraph1.Inlines.Add(new LineBreak());
                    paragraph1.Inlines.Add(new Run("Cédula: 2-0290-0936"));
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
                    paragraph4.Inlines.Add(new Run("Sub-Total: "));
                    paragraph4.Inlines.Add(venta.VentasDetalles.Sum<VentasDetalle>((Func<VentasDetalle, int>)(det => det.Artículo.Precio.Value)).ToString("C0"));
                    paragraph4.Inlines.Add(new LineBreak());
                    paragraph4.Inlines.Add(new Run("IVA: "));
                    paragraph4.Inlines.Add(venta.Impuesto.ToString("C0"));
                    paragraph4.Inlines.Add(new LineBreak());
                    paragraph4.Inlines.Add(new Run("Monto Total: "));
                    paragraph4.Inlines.Add(venta.Total.ToString("C0"));
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
