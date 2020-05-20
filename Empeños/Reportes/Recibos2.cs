using Empeños.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

                    var bimg = new BitmapImage();

                    bimg.BeginInit();
                    bimg.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), @"Recursos\Imágenes\Logo.png"), UriKind.Absolute);
                    bimg.EndInit();
                    image.Source = bimg;

                    doc = new FlowDocument
                    {
                        PageWidth = anchoPágina,
                        PagePadding = new Thickness(margenPágina),
                        PageHeight = double.NaN,
                        FontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana"),
                        FontSize = 13,
                        Blocks =
                        {
                            new BlockUIContainer(image),
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Center,
                                Inlines =
                                {
                                    new LineBreak(),
                                    new Run("Compra Venta & Casa de Empeño"), new LineBreak(),
                                    new Run("La Salvada"){FontSize = 18, FontWeight=FontWeights.Bold}, new LineBreak(),new LineBreak(),
                                    new Run("Porfirio Morales Mora"), new LineBreak(),
                                    new Run("Ced:2-290-936 Tel: 2474-0641"), new LineBreak(),
                                    new Run("Aguas Zarcas - San Carlos"), new LineBreak(), new LineBreak(),
                                    new Run("EMPEÑO NO. " + empeño.Código){ FontWeight = FontWeights.Bold, FontSize = 18}
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Left,
                                Inlines =
                                {
                                    new Run("Fecha  : " + empeño.Fecha.ToString("dd/MMM/yyyy")), new LineBreak(),
                                    new Run("Cliente: " + empeño.Cliente.NombreCompleto), new LineBreak(),
                                    new Run("Cédula : " + empeño.Código_Cliente), new LineBreak(),new LineBreak(),
                                    new Line{Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1}
                                }
                            }
                        }
                    };

                    foreach (var art in empeño.EmpeñosDetalles)
                    {
                        doc.Blocks.Add(new Paragraph { TextAlignment = TextAlignment.Left, Inlines = { new Run(art.Artículo.ToString()) } });
                        /*
                        doc.Blocks.Add(new Paragraph
                         {
                             TextAlignment = TextAlignment.Right,
                             Inlines = { new Run("Monto: " + art.Artículo.MontoPréstamo.ToString(formatoMoneda)) }
                         });
                         */
                    }

                    doc.Blocks.AddRange(new Block[]{
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Right,
                                Inlines = 
                                {
                                    /*new Line{Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1}, new LineBreak(), new LineBreak(),*/
                                    new Run("Total Préstamo: "), empeño.TotalMontoPréstamo.ToString(formatoMoneda)
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Justify,
                                FontSize = 10,
                                Inlines = 
                                {
                                    new LineBreak(), new LineBreak(),new LineBreak(), new LineBreak(),
                                    new Run("Firma:_____________________________"){FontSize = 12}, new LineBreak(), new LineBreak(),
                                    new Run("1. Todo préstamo, es hecho a un plazo de trés meses, por un mes de intereses cancelado renueva el plazo de vencimiento un més."),new LineBreak(),
                                    new Run("2. Tasa de interés del 10% mensual, cobrandose siempre un més como mínimo. Pagaderos por mes."),new LineBreak(),
                                    new Run("3. El no pago de intereses en 3 meses autoriza a la Salvada para que disponga de la prenda."),new LineBreak(),new LineBreak(),
                                    new Run("Autorizado mediante resolución # 11-97, de la Gaceta # 171 del viernes 5/Sep/1997.") {FontWeight = FontWeights.Bold}
                                }
                            }
                    });
                }
            }

            return doc;
        }

        public static FlowDocument ReciboDePago(string códigoEmpeño, string códigoCliente, string nombreCliente, int cuota, DateTime fecha, int intereses, int abono, int saldo)
        {
            Image image = new Image() { Stretch = Stretch.None };
            var bimg = new BitmapImage();

            bimg.BeginInit();
            bimg.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), @"Recursos\Imágenes\Logo.png"), UriKind.Absolute);
            bimg.EndInit();
            image.Source = bimg;

            var doc = new FlowDocument
            {
                PageWidth = anchoPágina,
                PagePadding = new Thickness(margenPágina),
                PageHeight = double.NaN,
                FontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana"),
                FontSize = 13,
                Blocks =
                {
                    new BlockUIContainer(image),
                    new Paragraph
                    {
                        TextAlignment = TextAlignment.Center,
                        Inlines =
                        {
                            new LineBreak(),
                            new Run("Compra Venta & Casa de Empeño"), new LineBreak(),
                            new Run("La Salvada"){FontSize = 18, FontWeight=FontWeights.Bold}, new LineBreak(),new LineBreak(),
                            new Run("Porfirio Morales Mora"), new LineBreak(),
                            new Run("Ced:2-290-936 Tel: 2474-0641"), new LineBreak(),
                            new Run("Aguas Zarcas - San Carlos"), new LineBreak(), new LineBreak(),
                            new Run((saldo == 0 ? "RETIRO" : "RENOVACIÓN") + " NO. " + códigoEmpeño){ FontWeight = FontWeights.Bold, FontSize = 18}
                        }
                    },
                    new Paragraph
                    {
                        TextAlignment = TextAlignment.Left,
                        Inlines =
                        {
                            new Run("Cliente: " + nombreCliente), new LineBreak(),
                            new Run("Cédula : " + códigoCliente), new LineBreak(),
                            new Run("Paga Desde: " + fecha.AddMonths(-1).ToString("dd/MMM/yyyy")), new LineBreak(),
                            new Run("Paga Hasta: " + fecha.ToString("dd/MMM/yyyy")), new LineBreak(),new LineBreak(),
                            new Line{Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1}, new LineBreak()
                        }
                    },
                    new Table
                    {
                        RowGroups =
                        {
                            new TableRowGroup
                            {
                                Rows =
                                {
                                    new TableRow
                                    {
                                        Cells =
                                        {
                                            new TableCell(new Paragraph { Inlines = { new Run("No. de Cuota : ") } }){TextAlignment = TextAlignment.Left},
                                            new TableCell(new Paragraph { Inlines = { new Run(cuota.ToString()) } }){TextAlignment = TextAlignment.Right}
                                        }
                                    },
                                    new TableRow
                                    {
                                        Cells =
                                        {
                                            new TableCell(new Paragraph { Inlines = { new Run("Intereses : ") } }){TextAlignment = TextAlignment.Left},
                                            new TableCell(new Paragraph { Inlines = { new Run(intereses.ToString(formatoMoneda)) } }){TextAlignment = TextAlignment.Right}
                                        }
                                    },
                                    new TableRow
                                    {
                                        Cells =
                                        {
                                            new TableCell(new Paragraph { Inlines = { new Run("Abono : ") } }){TextAlignment = TextAlignment.Left},
                                            new TableCell(new Paragraph { Inlines = { new Run(abono.ToString(formatoMoneda)) } }){TextAlignment = TextAlignment.Right}
                                        }
                                    },
                                    new TableRow {},
                                    new TableRow
                                    {
                                        Cells =
                                        {
                                            new TableCell(new Paragraph { Inlines = { new Run("Saldo : ") } }){TextAlignment = TextAlignment.Left},
                                            new TableCell(new Paragraph { Inlines = { new Run(saldo.ToString(formatoMoneda)) } }){TextAlignment = TextAlignment.Right}
                                        }
                                    }
                                }
                            }
                        }
                    }                    
                }
            };

            if (saldo > 0)
                doc.Blocks.Add(new Paragraph
                {
                    TextAlignment = TextAlignment.Justify,
                    FontSize = 10,
                    Inlines = 
                        {
                            new LineBreak(), new LineBreak(),new LineBreak(), new LineBreak(),
                            new Run("Firma:_____________________________"){FontSize = 12}, new LineBreak(), new LineBreak(),
                            new Run("1. Todo préstamo, es hecho a un plazo de trés meses, por un mes de intereses cancelado renueva el plazo de vencimiento un més."),new LineBreak(),
                            new Run("2. Tasa de interés del 10% mensual, cobrandose siempre un més como mínimo. Pagaderos por mes."),new LineBreak(),
                            new Run("3. El no pago de intereses en 3 meses autoriza a la Salvada para que disponga de la prenda."),new LineBreak(),new LineBreak(),
                            new Run("Autorizado mediante resolución # 11-97, de la Gaceta # 171 del viernes 5/Sep/1997.") {FontWeight = FontWeights.Bold}
                        }
                });
            else
                doc.Blocks.Add(new Paragraph
                {
                    TextAlignment = TextAlignment.Justify,
                    FontSize = 10,
                    Inlines = 
                        {
                            new LineBreak(), new LineBreak(),new LineBreak(), new LineBreak(),
                            new Run("Firma:_____________________________"){FontSize = 12}, new LineBreak(), new LineBreak(),
                            new Run("No se aceptarán reclamos sobre artículos retirados, una vez que salgan de nuestras instalaciones."),new LineBreak(),new LineBreak(),
                            new Run("Autorizado mediante resolución # 11-97, de la Gaceta # 171 del viernes 5/Sep/1997.") {FontWeight = FontWeights.Bold}
                        }
                });

            return doc;
        }

        public static FlowDocument ReciboDeVenta(int códigoVenta)
        {
            FlowDocument doc = null;

            using (var bd = new EmpeñosDataContext())
            {
                var venta = bd.Ventas.SingleOrDefault(ven => ven.Código == códigoVenta);

                if (venta != null)
                {
                    Image image = new Image() { Stretch = Stretch.None };
                    var bimg = new BitmapImage();

                    bimg.BeginInit();
                    bimg.UriSource = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), @"Recursos\Imágenes\Logo.png"), UriKind.Absolute);
                    bimg.EndInit();
                    image.Source = bimg;

                    doc = new FlowDocument
                    {
                        PageWidth = anchoPágina,
                        PagePadding = new Thickness(margenPágina),
                        PageHeight = double.NaN,
                        FontFamily = new FontFamily("Consolas, Comic Sans MS, Verdana"),
                        FontSize = 13,
                        Blocks =
                        {
                            new BlockUIContainer(image),
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Center,
                                Inlines =
                                {
                                    new LineBreak(),
                                    new Run("Compra Venta & Casa de Empeño"), new LineBreak(),
                                    new Run("La Salvada"){FontSize = 18, FontWeight=FontWeights.Bold}, new LineBreak(),new LineBreak(),
                                    new Run("Porfirio Morales Mora"), new LineBreak(),
                                    new Run("Ced:2-290-936 Tel: 2474-0641"), new LineBreak(),
                                    new Run("Aguas Zarcas - San Carlos"), new LineBreak(), new LineBreak(),
                                    new Run("FACTURA DE VENTA NO. " + venta.Código){ FontWeight = FontWeights.Bold, FontSize = 18}
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Left,
                                Inlines =
                                {
                                    new Run("Fecha  : " + venta.Fecha.ToString("dd/MMM/yyyy")), new LineBreak(),
                                    new Run("Cliente: " + venta.Cliente.NombreCompleto), new LineBreak(),
                                    new Run("Cédula : " + venta.Código_Cliente), new LineBreak(),new LineBreak(),
                                    new Line{Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1}
                                }
                            }
                        }
                    };

                    foreach (var art in venta.VentasDetalles)
                    {
                        doc.Blocks.Add(new Paragraph { TextAlignment = TextAlignment.Left, Inlines = { new Run(art.Artículo.ToString()) } });
                        /*
                        doc.Blocks.Add(new Paragraph
                         {
                             TextAlignment = TextAlignment.Right,
                             Inlines = { new Run("Monto: " + art.Artículo.MontoPréstamo.ToString(formatoMoneda)) }
                         });
                         */
                    }

                    doc.Blocks.AddRange(new Block[]{
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Right,
                                Inlines = 
                                {
                                    /*new Line{Stretch = Stretch.Fill, Stroke = new SolidColorBrush(Colors.Black), X2 = 1}, new LineBreak(), new LineBreak(),*/
                                    new Run("Monto Total: "), venta.VentasDetalles.Sum(det => det.Artículo.Precio.Value).ToString(formatoMoneda)
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Justify,
                                FontSize = 10,
                                Inlines = 
                                {
                                    new LineBreak(), new LineBreak(),new LineBreak(), new LineBreak(),
                                    new Run("1. Favor revisar los artículos antes de salir del local, no se aceptan devoluciones.")
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Center,
                                Inlines =
                                {
                                   new Run("GRACIAS POR PREFERIRNOS.")
                                }
                            },
                            new Paragraph
                            {
                                TextAlignment = TextAlignment.Center,
                                Inlines                                    =
                                {
                                    new Run("Autorizado mediante resolución # 11-97 de la D.G.T.D.") {FontWeight = FontWeights.Bold}, new LineBreak(), new LineBreak()
                                }
                            }
                        });
                }
            }

            return doc;
        }


        public static void Imprimir(string título, FlowDocument recibo, bool previsualizar = false)
        {
            if (recibo != null)
            {
                var dlgImprimir = new PrintDialog();
                if (!previsualizar || dlgImprimir.ShowDialog() == true)
                    dlgImprimir.PrintDocument((recibo as IDocumentPaginatorSource).DocumentPaginator, título);
            }
        }
    }
}
