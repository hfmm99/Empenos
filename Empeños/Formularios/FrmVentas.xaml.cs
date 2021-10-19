using Empeños.Clases;
using Empeños.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Collections.ObjectModel;

namespace Empeños.Formularios
{
    public partial class FrmVentas : Window
    {
        int? códigoVenta;

        Parámetro parámetros;

        List<Artículo> listaArtículos = new List<Artículo>();
        ObservableCollection<VentasAbono> abonos;

        public FrmVentas()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        public FrmVentas(int pCódigoVenta)
            : this()
        {
            códigoVenta = pCódigoVenta;
        }

        public bool ImprimirAlGuardar
        {
            get { return chkImprimirAlGuardar.IsChecked == true; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var bd = new EmpeñosDataContext();
            parámetros = bd.Parámetros.FirstOrDefault();

            dtpFecha.SelectedDate = DateTime.Today;

            txtCódigo.Focus();

            if (códigoVenta.HasValue)
            {
                var venta = bd.Ventas.SingleOrDefault(c => c.Código == códigoVenta);
                if (venta != null)
                {
                    txtCódigo.Text = venta.Código.ToString();
                    txtCódigo.IsReadOnly = true;
                    chkImprimirAlGuardar.Visibility = Visibility.Collapsed;
                    dtpFecha.SelectedDate = venta.Fecha;
                    cmbEstado.SelectedIndex = (int)venta.Estado;
                    txtCódigo.IsEnabled = false;
                    txtMontoAPagar.Text = venta.Total.ToString();

                    if (venta.Cliente != null)
                    {
                        txtClientes.SelectedItem = new KeyValuePair<string, string>(venta.Cliente.Código, venta.Cliente.NombreCompleto);
                        txtClientes.Text = venta.Cliente.NombreCompleto;
                    }

                    grdArtículos.ItemsSource = null;

                    foreach (var det in venta.VentasDetalles)
                        listaArtículos.Add(det.Artículo);

                    grdArtículos.ItemsSource = listaArtículos;

                    ActualizarTotales();

                    dtpFecha.Focus();
                }
            }

            grdArtículos.ItemsSource = listaArtículos;
        }

        private void pago_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Intereses" || e.PropertyName == "Abono")
                ActualizarTotales();
        }

        private void txtClientes_Populating(object sender, PopulatingEventArgs e)
        {
            using (var bd = new EmpeñosDataContext())
            {
                txtClientes.ItemsSource = bd.Clientes
                           .Where(c => c.Código.Contains(txtClientes.Text) || c.NombreCompleto.Contains(txtClientes.Text))
                           .Select(c => new KeyValuePair<string, string>(c.Código, c.NombreCompleto)).Take(15);
            }
        }

        private void txtArtículos_Populating(object sender, PopulatingEventArgs e)
        {
            using (var bd = new EmpeñosDataContext())
            {
                txtArtículos.ItemsSource = bd.Artículos
                                             .Where(a => a.Estado == (byte)EstadosActículos.Quedado && (a.Código.Contains(txtArtículos.Text) || a.Nombre.Contains(txtArtículos.Text) || a.Artículos_Características.Any(ac => ac.Valor.Contains(txtArtículos.Text))))
                                             .Select(a => new KeyValuePair<string, string>(a.Código, a.DescripciónExtendida))
                                             .Take(15);
            }
        }

        private void btnLimpiarCliente_Click(object sender, RoutedEventArgs e)
        {
            // txtClientes.IsEnabled = true;
            txtClientes.SelectAll();
        }

        private void lblCliente_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrmClientes frmClientes;

            if (txtClientes.SelectedItem != null)
            {
                var cliente = (KeyValuePair<string, string>)txtClientes.SelectedItem;
                frmClientes = new FrmClientes(cliente.Key) { Owner = this };
            }
            else
                frmClientes = new FrmClientes() { Owner = this };

            if (frmClientes.ShowDialog() == true)
                if (frmClientes.Cliente != null)
                {
                    txtClientes.SelectedItem = new KeyValuePair<string, string>(frmClientes.Cliente.Código, frmClientes.Cliente.NombreCompleto);
                    txtClientes.Text = frmClientes.Cliente.NombreCompleto;
                }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarVenta())
            {
                using (var bd = new EmpeñosDataContext())
                {
                    bool insertando = false;

                    var venta = bd.Ventas.SingleOrDefault(v => v.Código == txtCódigo.AsInt);

                    if (venta == null)
                    {
                        venta = new Venta { Código = txtCódigo.AsInt };
                        bd.Ventas.InsertOnSubmit(venta);
                        insertando = true;
                    }

                    venta.Código_Cliente = ((KeyValuePair<string, string>)txtClientes.SelectedItem).Key;
                    venta.Fecha = dtpFecha.SelectedDate.Value;
                    venta.Notas = txtNotasVentas.Text;
                    venta.Estado = Convert.ToByte(cmbEstado.SelectedIndex);

                    if (!insertando)
                    {
                        bd.VentasDetalles.DeleteAllOnSubmit(venta.VentasDetalles);
                        bd.VentasAbonos.DeleteAllOnSubmit(venta.VentasAbonos);
                    }

                    for (int cont = 0; cont < listaArtículos.Count; cont++)
                    {
                        Artículo art = bd.Artículos.SingleOrDefault(a => a.Código == listaArtículos[cont].Código);

                        if (art != null)
                        {
                            art.Precio = listaArtículos[cont].Precio;
                            art.Estado = (byte)EstadosActículos.Vendido;

                            venta.VentasDetalles.Add(new VentasDetalle { Código_Venta = venta.Código, Código_Artículo = art.Código });
                        }
                        else
                        {
                            MessageBox.Show("El artículo " + listaArtículos[cont].Código + " ya no existe en la base de datos");
                            return;
                        }
                    }

                    int cuota = 0;
                    venta.VentasAbonos.Add(new VentasAbono { Cuota = ++cuota, Fecha = DateTime.Now, Monto = txtTotalMontoVenta.AsInt });
                    venta.Impuesto = txtIVA.AsDecimal;
                    venta.Total = (int)txtMontoAPagar.AsDecimal;

                    bd.SubmitChanges();

                    this.DialogResult = true;
                }
            }
        }

        private bool ValidarVenta()
        {
            if (txtClientes.SelectedItem == null)
            {
                txtClientes.Text = string.Empty;
                MessageBox.Show("Debe seleccionar un cliente, o ingresar a la pantalla de clientes a insertar uno");
                txtClientes.Focus();
                return false;
            }

            if (txtTotalMontoVenta.AsInt == 0)
            {
                MessageBox.Show("Debe ingresar al menos un artículo y asignarles precio");
                txtArtículos.Focus();
                return false;
            }

            if (listaArtículos.Any(a => a.Precio == 0))
            {
                MessageBox.Show("Todos los artículos deben tener un precio de venta");
                grdArtículos.Focus();
                return false;
            }

            if (dtpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe ingresar la fecha de la venta");
                dtpFecha.Focus();
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtCódigo_Loaded(object sender, RoutedEventArgs e)
        {
            if (txtCódigo.AsInt == 0)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    int últimoCódigo = bd.Ventas.Select(emp => (int?)emp.Código).Max() ?? 0;
                    txtCódigo.AsInt = últimoCódigo + 1;
                    txtCódigo.SelectAll();
                }
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (txtClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente");
                return;
            }

            if (gridPagos.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un pago");
                return;
            }

            var pago = gridPagos.SelectedItem as EmpeñosPago;
            var cliente = (KeyValuePair<string, string>)txtClientes.SelectedItem;

            int saldo = txtTotalMontoPréstamo.AsInt - gridPagos.Items.OfType<EmpeñosPago>().Where(p => p.Cuota <= gridPagos.SelectedIndex + 1).Sum(p => p.Abono);

            Recibos.Imprimir("Imprimiento Recibo de Pago", Recibos.ReciboDePago(txtCódigo.AsInt.ToString(), cliente.Key, cliente.Value, gridPagos.SelectedIndex + 1, pago.FechaCuota, pago.Intereses, pago.Abono, saldo));
             * */
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = this.DialogResult != true && MessageBox.Show("¿Está seguro que desea salir, sin guardar los cambios?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.No;
        }

        private void ActualizarTotales()
        {
            var bd = new EmpeñosDataContext();
            parámetros = bd.Parámetros.FirstOrDefault();
            /*
            var pagos = gridPagos.Items.OfType<EmpeñosPago>().ToArray();

            if (pagos.Length > 0)
            {
                txtTotalInteresesCancelados.AsInt = pagos.Sum(p => p.Intereses);

                int totalDePréstamoCancelado = pagos.Sum(p => p.Abono);

                txtTotalDePréstamoCancelado.AsInt = totalDePréstamoCancelado;

                int saldo = txtTotalMontoPréstamo.AsInt - totalDePréstamoCancelado;
                txtSaldoDelPréstamo.AsInt = saldo;
                btnNuevoPago.IsEnabled = btnRetirar.IsEnabled = saldo > 0;
                cmbEstado.SelectedIndex = saldo == 0 ? (int)EstadosEmpeño.Retirado : (int)EstadosEmpeño.Activo;

                dtpFechaVencimiento.SelectedDate = pagos.Max(p => p.FechaCuota).AddMonths(parámetros.Plazo);
            }
            else
            {
                txtTotalInteresesCancelados.AsInt = txtTotalDePréstamoCancelado.AsInt = 0;
                txtSaldoDelPréstamo.AsInt = txtTotalMontoPréstamo.AsInt;
                dtpFechaVencimiento.SelectedDate = dtpFecha.SelectedDate.Value.AddMonths(parámetros.Plazo);
            }

            TimeSpan díasRestantes = dtpFechaVencimiento.SelectedDate.Value - DateTime.Today;

            if (díasRestantes.Days > 30)
                ctrlIndicador.Estado = Controles.EstadoIndicador.Verde;
            else if (díasRestantes.Days > 3)
                ctrlIndicador.Estado = Controles.EstadoIndicador.Amarillo;
            else
                ctrlIndicador.Estado = Controles.EstadoIndicador.Rojo;
             * */
<<<<<<< HEAD
           var suma = listaArtículos.Sum(art => art.Precio);
           var impuesto = 0;
           var total = 0;

           DateTime fecha_x = new DateTime(2021, 2, 28); // Este codigo de fecha se agrego para no hacer cambios en BD
           DateTime fecha_y = new DateTime(2020, 06, 27); // En este fecha aproximadamente se hicieron los cambios en Git
           DateTime fecha_z = new DateTime(2021, 10, 19); // En esta fecha se volvio a poner el IVA en 13%
           int result1 = DateTime.Compare((DateTime)dtpFecha.SelectedDate , fecha_x); // se paso de 0% a 13% y luego a 4% de IVA
           int result2 = DateTime.Compare((DateTime)dtpFecha.SelectedDate, fecha_y);
           int result3 = DateTime.Compare((DateTime)dtpFecha.SelectedDate, fecha_z);

            if (result1 >= 0)
            {
                impuesto = (int)(suma * 0.04);
            }
            else if (result2 > 0)
            {
                impuesto = (int)(suma * 0.13);
            }
            else if (result2 <= 0)
            {
                impuesto = 0;
            }
            else if (result3 >= 0 ) {
                impuesto = (int)(suma * parámetros.IVA); // cambiar a 13% el IVA en la tabla de parametros
                
            }
            total = (int)(suma + impuesto);
=======
            var suma = listaArtículos.Sum(art => art.Precio);
            var impuesto = suma * parámetros.IVA;
            var total = suma + impuesto;
>>>>>>> 8282796da909b9cabbf1065b8bd4c5692d87cad0
            txtIVA.Text = impuesto.ToString();
            txtTotalMontoVenta.Text = suma.ToString();
            txtMontoAPagar.Text = total.ToString();
        }

        private void btnAgregarArtículo_Click(object sender, RoutedEventArgs e)
        {
            if (txtArtículos.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un artículo");
                return;
            }

            if (listaArtículos.Any(a => a.Código == ((KeyValuePair<string, string>)txtArtículos.SelectedItem).Key))
            {
                MessageBox.Show("El artículo seleccionado no puede ser ingresado varias veces a la factura");
                return;
            }

            var bd = new EmpeñosDataContext();
            var art = bd.Artículos.SingleOrDefault(a => a.Código == ((KeyValuePair<string, string>)txtArtículos.SelectedItem).Key);

            if (art != null)
            {
                grdArtículos.ItemsSource = null;
                art.Precio = 0;
                listaArtículos.Add(art);
                grdArtículos.ItemsSource = listaArtículos;
                txtArtículos.Text = string.Empty;
                grdArtículos.SelectedItem = art;
                grdArtículos.BeginEdit();
                ActualizarTotales();
            }
        }

        private void btnBorrarArtículo_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desea quitar el artículo seleccionado de la factura de Venta?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int indiceArt = grdArtículos.SelectedIndex;
                grdArtículos.ItemsSource = null;
                listaArtículos.RemoveAt(indiceArt);
                grdArtículos.ItemsSource = listaArtículos;
                ActualizarTotales();
            }
        }

        private void grdArtículos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EasyTimer.SetTimeout(() =>
            {
                ActualizarTotales();
            }, 500);
        }

        private void grdArtículos_CurrentCellChanged(object sender, EventArgs e)
        {
            ActualizarTotales();
        }

        private void grdArtículos_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            ActualizarTotales();
        }

        private void rbTipo_Checked(object sender, RoutedEventArgs e)
        {
            if (rbTipoApartado != null)
                tabAbonos.Visibility = rbTipoApartado.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
        }

        private void GridAbonos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridApartado.SelectedItem != null)
            {
                var abono = gridApartado.SelectedItem as VentasAbono;

                if (abono != null)
                {
                    var frmVentas = new FrmVentas();

                    if (frmVentas.ShowDialog() == true)
                    {
                        //abono.Fecha = frmVentas.abon;
                        //abono.FechaCuota = frmCuota.FechaCuota;
                    }
                }
            }
        }

        private void btnAgregarAbono(object sender, RoutedEventArgs e)
        {
            var frmAbono = new FrmAbono(int.Parse(txtCódigo.Text), 0, DateTime.Now, 0);

            try
            {
                //inkFirma.SetTabletState(1);

                if (frmAbono.ShowDialog() == true)
                {
                    var abono = new VentasAbono {Código_Venta = txtCódigo.AsInt, Cuota = abonos.Count, Fecha = frmAbono.Fecha, Monto = frmAbono.Monto };

                    abono.PropertyChanged += pago_PropertyChanged;
                    abonos.Add(abono);
                    gridApartado.SelectedItem = abono;
                    //ActualizarTotales();
                }
            }
            finally
            {
              //  inkFirma.SetTabletState(1);
            }
        }
    }
}
