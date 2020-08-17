using Empeños.Clases;
using Empeños.Datos;
using Empeños.Reportes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Empeños.Formularios
{
    /// <summary>
    /// Lógica de interacción para FrmEmpeños.xaml
    /// </summary>
    public partial class FrmEmpeños : Window
    {
        int? códigoEmpeño;
        bool reempeñar;
        bool modificandoArtículo = false;
        List<CaracterísticaValor> características;
        Parámetro parámetros;
        ObservableCollection<EmpeñosPago> pagos;

        public FrmEmpeños()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        public FrmEmpeños(int pCódigoEmpeño, bool pReempeñar = false)
            : this()
        {
            códigoEmpeño = pCódigoEmpeño;
            reempeñar = pReempeñar;
            
        }

        public bool ImprimirAlGuardar
        {
            get { return chkImprimirAlGuardar.IsChecked == true; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Configurar Firma

            inkFirma.SetSigCompressionMode(2);
            inkFirma.SetEncryptionMode(2);
            inkFirma.AutoKeyStart();
            inkFirma.SetAutoKeyData("La Salvada");
            inkFirma.AutoKeyFinish();

            #endregion

            características = new List<CaracterísticaValor>();
            pagos = new ObservableCollection<EmpeñosPago>();
            gridPagos.ItemsSource = pagos;

            var bd = new EmpeñosDataContext();
            parámetros = bd.Parámetros.FirstOrDefault();

            cmbCategorías.Items.Insert(0, new KeyValuePair<int?, string>(null, string.Empty));

            foreach (var cat in bd.Categorías.OrderBy(cat => cat.Nombre))
                cmbCategorías.Items.Add(new KeyValuePair<int?, string>(cat.Código, cat.Nombre));

            cmbCategorías.SelectedIndex = 0;


            dtpFecha.SelectedDate = DateTime.Today;
            pnlDetalleArtículo.Visibility = Visibility.Collapsed;
            txtCódigo.Focus();

            if (códigoEmpeño.HasValue)
            {
                var empeño = bd.Empeños.SingleOrDefault(c => c.Código == códigoEmpeño);
                if (empeño != null)
                {
                    if (!reempeñar)
                    {
                        txtCódigo.Text = empeño.Código.ToString();
                        txtCódigo.IsReadOnly = true;
                        chkImprimirAlGuardar.Visibility = Visibility.Collapsed;
                        dtpFecha.SelectedDate = empeño.Fecha;
                        cmbEstado.SelectedIndex = (int)empeño.Estado;
                        gridArtículos.IsEnabled = pnlListaPagos.IsEnabled = btnGuardar.IsEnabled = empeño.Estado != (byte)EstadosEmpeño.Quedado;
                        txtCódigo.IsEnabled = false; ///*****///
                        tabPagos.IsEnabled = true;
                    }

                    if (empeño.Cliente != null)
                    {
                        txtClientes.SelectedItem = new KeyValuePair<string, string>(empeño.Cliente.Código, empeño.Cliente.NombreCompleto);
                        txtClientes.Text = empeño.Cliente.NombreCompleto;
                    }

                    txtPlazo.AsInt = empeño.Plazo;
                    txtPorcentajeIntereses.AsDecimal = empeño.PorcentajeInterés;
                    txtTotalMontoPréstamo.AsInt = empeño.TotalMontoPréstamo;
                    txtNotasEmpeño.Text = empeño.Notas;

                    foreach (EmpeñosDetalle det in empeño.EmpeñosDetalles)
                    {
                        lstArtículos.Items.Add(det.Artículo);
                        det.Artículo.Artículos_Características.ToArray();
                        



                    }

                    if (reempeñar) {
                        txtPlazo.AsInt = parámetros.Plazo;
                        txtPorcentajeIntereses.AsDecimal = Math.Round(parámetros.PorcentajeInterés, 2);
                    }

                    if (!reempeñar)
                    {
                        pagos.Clear();
                        foreach (var pago in empeño.EmpeñosPagos)
                        {
                            pago.PropertyChanged += pago_PropertyChanged;
                            pagos.Add(pago);
                        }
                    }
                    ActualizarTotales();

                    #region Firma

                    // Cargar Firma
                    if (empeño.Firma != null && empeño.Firma.Length > 0)
                        inkFirma.SetSigString(empeño.Firma);

                    inkFirma.SetTabletState(1);

                    #endregion

                    dtpFecha.Focus();

                }
            }
            else
            {
                txtPlazo.AsInt = parámetros.Plazo;
                txtPorcentajeIntereses.AsDecimal = Math.Round(parámetros.PorcentajeInterés, 2);
            }
            if (txtPlazo.Text != "1") { labelMeses.Content = "meses"; }
        }

        private void pago_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Intereses" || e.PropertyName == "Abono")
                ActualizarTotales();
        }

        private void btnNuevoArtículo_Click(object sender, RoutedEventArgs e)
        {
            NuevoArtículo();
        }

        private void btnModificarArtículo_Click(object sender, RoutedEventArgs e)
        {
            ModificarArtículo();
        }

        private void btnBorrarArtículo_Click(object sender, RoutedEventArgs e)
        {
            BorrarArtículo();
        }

        private void btnGuardarArtículo_Click(object sender, RoutedEventArgs e)
        {
            GuardarArtículo();
        }

        private bool GuardarArtículo()
        {
            if (ValidarArtículo())
            {
                Artículo artículo = null;

                if (modificandoArtículo && lstArtículos.SelectedItem != null)
                    artículo = lstArtículos.SelectedItem as Artículo;
                else
                    artículo = new Artículo();

                artículo.Nombre = txtNombre.Text;
                artículo.Costo = txtMontoPréstamo.AsInt;
                artículo.Código_Categoría = (int?)cmbCategorías.SelectedValue;

                artículo.Artículos_Características.Clear();

                foreach (CaracterísticaValor car in gridCaracterísticas.Items)
                    artículo.Artículos_Características.Add(new Artículos_Característica { Código_Característica = car.CódigoCaracterística, Valor = car.Valor });

                artículo.Notas = txtNotas.Text;

                if (!modificandoArtículo)
                {
                    lstArtículos.Items.Add(artículo);
                    lstArtículos.SelectedItem = artículo;
                }
                else
                    lstArtículos.Items.Refresh();

                ActualizarTotalMontoPréstamo();

                Animaciones.DeslizarElementos(this, pnlListaArtículos, pnlDetalleArtículo);
                return true;
            }
            return false;
        }

        private void Guardar(bool cerrar)
        {
            if (pnlDetalleArtículo.IsVisible)
                if (!GuardarArtículo())
                    return;

            if (ValidarEmpeño())
            {
                using (var bd = new EmpeñosDataContext())
                {
                    bool insertando = false;

                    var empeño = bd.Empeños.SingleOrDefault(em => em.Código == txtCódigo.AsInt);

                    if (empeño == null)
                    {
                        empeño = new Empeño
                        {
                            Código = txtCódigo.AsInt,
                            PorcentajeInterés = txtPorcentajeIntereses.AsDecimal,
                            TotalMontoPréstamo = txtTotalMontoPréstamo.AsInt
                        };
                        bd.Empeños.InsertOnSubmit(empeño);
                        insertando = true;
                    }

                    empeño.Estado = Convert.ToByte(cmbEstado.SelectedIndex);

                    if (empeño.Estado != (byte)EstadosEmpeño.Quedado)
                    {
                        string códigoCliente = ((KeyValuePair<string, string>)txtClientes.SelectedItem).Key;

                        if (!insertando && empeño.Código_Cliente != códigoCliente && MessageBox.Show(string.Format("El cliente del empeño cambió, antes era {0:G} y ahora {1:G}, está seguro?", empeño.Código_Cliente, códigoCliente), "Pregunta", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            return;

                        empeño.Código_Cliente = códigoCliente;
                        empeño.Fecha = dtpFecha.SelectedDate.Value;
                        empeño.Plazo = txtPlazo.AsInt;
                        empeño.Notas = txtNotasEmpeño.Text;

                        if (inkFirma.NumberOfTabletPoints() == 0)
                            empeño.Firma = null;
                        else
                            empeño.Firma = inkFirma.GetSigString();

                        if (!insertando)
                        {
                            foreach (var detalle in empeño.EmpeñosDetalles)
                            {
                                bd.Artículos_Características.DeleteAllOnSubmit(detalle.Artículo.Artículos_Características);
                                bd.Artículos.DeleteOnSubmit(detalle.Artículo);
                                bd.EmpeñosDetalles.DeleteOnSubmit(detalle);
                            }
                            bd.EmpeñosPagos.DeleteAllOnSubmit(empeño.EmpeñosPagos);
                        }

                        for (int cont = 0; cont < lstArtículos.Items.Count; cont++)
                        {
                            var art = lstArtículos.Items[cont] as Artículo;

                            var nuevoArtículo = new Artículo
                            {
                                Código = "E" + txtCódigo.AsInt.ToString() + "-" + (cont + 1).ToString(),
                                Nombre = art.Nombre,
                                Código_Categoría = art.Código_Categoría,
                                Costo = art.Costo,
                                Estado = art.Estado,
                                Notas = art.Notas
                            };

                            foreach (var caract in art.Artículos_Características)
                                nuevoArtículo.Artículos_Características.Add(new Artículos_Característica { Código_Característica = caract.Código_Característica, Valor = caract.Valor });

                            empeño.EmpeñosDetalles.Add(new EmpeñosDetalle { Artículo = nuevoArtículo });
                        }

                        int cuota = 0;
                        foreach (var pago in pagos)
                            empeño.EmpeñosPagos.Add(new EmpeñosPago { Cuota = ++cuota, FechaPago = pago.FechaPago, FechaCuota = pago.FechaCuota, Intereses = pago.Intereses, Abono = pago.Abono, Firma = pago.Firma?.Length > 0 ? pago.Firma : null });
                    }

                    bd.SubmitChanges();

                    if (cerrar)
                        this.DialogResult = true;
                }
            }
        }

        private void ActualizarTotalMontoPréstamo()
        {
            txtTotalMontoPréstamo.AsInt = (from Artículo art in lstArtículos.Items select (int?)art.Costo).Sum() ?? 0;

            ActualizarTotales();
        }

        private void txtClientes_Populating(object sender, PopulatingEventArgs e)
        {
            using (var bd = new EmpeñosDataContext())
                txtClientes.ItemsSource = bd.Clientes
                    .Where(c => c.Código.Contains(txtClientes.Text) || c.NombreCompleto.Contains(txtClientes.Text))
                    .Select(c => new KeyValuePair<string, string>(c.Código, c.NombreCompleto)).Take(10);
        }

        private void btnLimpiarCliente_Click(object sender, RoutedEventArgs e)
        {
            // txtClientes.IsEnabled = true;
            txtClientes.SelectAll();
        }

        private void lstArtículos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ModificarArtículo();
        }

        private void lstArtículos_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ModificarArtículo();
            else if (e.Key == Key.Delete)
                BorrarArtículo();
        }

        private void NuevoArtículo()
        {
            LimpiarArtículo();
            Animaciones.DeslizarElementos(this, pnlListaArtículos, pnlDetalleArtículo);
            txtNombre.Focus();
            modificandoArtículo = false;
        }

        private bool ValidarArtículo()
        {
            if (txtNombre.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debe ingresar el nombre del artículo");
                return false;
            }
            if (txtMontoPréstamo.AsInt <= 0)
            {
                MessageBox.Show("Debe ingresar un monto de préstamo para el artículo, mayor a cero");
                return false;
            }
            return true;
        }

        private void LimpiarArtículo()
        {
            txtNombre.Clear();
            txtMontoPréstamo.Clear();
            cmbCategorías.SelectedIndex = 0;
            gridCaracterísticas.ItemsSource = null;
            txtNotas.Clear();
        }

        private void ModificarArtículo()
        {
            if (lstArtículos.SelectedItem == null)
                MessageBox.Show("Debe seleccionar un artículo de la lista");
            else
            {
                var artículo = lstArtículos.SelectedItem as Artículo;

                txtNombre.Text = artículo.Nombre;
                txtMontoPréstamo.AsInt = artículo.Costo;
                cmbCategorías.SelectedValue = artículo.Código_Categoría;
                cmbCategorías_SelectionChanged(cmbCategorías, null);

                foreach (Artículos_Característica car in artículo.Artículos_Características)
                {
                    var caract = gridCaracterísticas.Items.OfType<CaracterísticaValor>().SingleOrDefault(c => c.CódigoCaracterística == car.Código_Característica);

                    if (caract != null)
                        caract.Valor = car.Valor.ToString();
                }

                txtNotas.Text = artículo.Notas;
                Animaciones.DeslizarElementos(this, pnlListaArtículos, pnlDetalleArtículo);
                txtNombre.Focus();
                modificandoArtículo = true;
            }
        }

        private void BorrarArtículo()
        {
            if (lstArtículos.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un artículo de la lista");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea borrar el artículo seleccionado?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                lstArtículos.Items.Remove(lstArtículos.SelectedItem);
                ActualizarTotalMontoPréstamo();
            }
        }

        private void btnCancelarArtículo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarArtículo();
            Animaciones.DeslizarElementos(this, pnlListaArtículos, pnlDetalleArtículo);
        }

        private void lblCliente_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrmClientes frmClientes;

            if (txtClientes.SelectedItem != null)
            {
                var cliente = (KeyValuePair<string, string>)txtClientes.SelectedItem;
                frmClientes = new FrmClientes(cliente.Key);
            }
            else
                frmClientes = new FrmClientes();

            if (frmClientes.ShowDialog() == true)
                if (frmClientes.Cliente != null)
                {
                    txtClientes.SelectedItem = new KeyValuePair<string, string>(frmClientes.Cliente.Código, frmClientes.Cliente.NombreCompleto);
                    txtClientes.Text = frmClientes.Cliente.NombreCompleto;
                }
        }

        private void cmbCategorías_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridCaracterísticas.ItemsSource = null;
            características.Clear();

            if (cmbCategorías.SelectedValue != null)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    foreach (var car in bd.Categorías.First(c => c.Código == (int)cmbCategorías.SelectedValue).Categorías_Características)
                        características.Add(new CaracterísticaValor { CódigoCaracterística = car.Código_Característica, NombreCaracterística = car.Característica.Nombre, Valor = string.Empty });
                }
                gridCaracterísticas.ItemsSource = características;
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            Guardar(true);
        }

        private bool ValidarEmpeño()
        {
            if (txtClientes.SelectedItem == null || string.IsNullOrEmpty(((KeyValuePair<string, string>)txtClientes.SelectedItem).Key))
            {
                txtClientes.Text = string.Empty;
                MessageBox.Show("Debe seleccionar un cliente, o ingresar a la pantalla de clientes a insertar uno");
                txtClientes.Focus();
                return false;
            }

            if (txtTotalMontoPréstamo.AsInt == 0)
            {
                MessageBox.Show("Debe ingresar al menos un artículo");
                btnNuevoArtículo.Focus();
                return false;
            }

            if (dtpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe ingresar la fecha inicial del empeño");
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
                    int últimoCódigo = bd.Empeños.Select(emp => (int?)emp.Código).Max() ?? 0;
                    txtCódigo.AsInt = últimoCódigo + 1;
                    txtCódigo.SelectAll();
                }
            }
        }

        private void btnNuevoPago_Click(object sender, RoutedEventArgs e)
        {
            if (dtpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe ingresar la fecha del empeño.");
                return;
            }

            var fechaCuota = DateTime.Now;

            if (pagos.Count == 0)
                fechaCuota = dtpFecha.SelectedDate.Value.AddMonths(1);
            else
            {
                var últimoPago = pagos.Last();
                fechaCuota = últimoPago.FechaCuota.AddMonths(1);
            }

            var frmCuota = new FrmCuota(txtCódigo.AsInt, sender == btnRetirar, DateTime.Now, fechaCuota, Convert.ToInt32(txtSaldoDelPréstamo.AsInt * txtPorcentajeIntereses.AsDecimal / 100), (sender == btnRetirar) ? txtTotalMontoPréstamo.AsInt - pagos.Sum(p => p.Abono) : 0);

            try
            {
                inkFirma.SetTabletState(1);

                if (frmCuota.ShowDialog() == true)
                {
                    var pago = new EmpeñosPago { Cuota = pagos.Count, FechaPago = frmCuota.FechaPago, FechaCuota = frmCuota.FechaCuota, Intereses = frmCuota.Intereses, Abono = frmCuota.Abono, Firma = frmCuota.Firma };

                    pago.PropertyChanged += pago_PropertyChanged;
                    pagos.Add(pago);
                    gridPagos.SelectedItem = pago;
                    ActualizarTotales();
                }
            }
            finally
            {
                inkFirma.SetTabletState(1);
            }
        }

        private void btnBorrarPago_Click(object sender, RoutedEventArgs e)
        {
            if (gridPagos.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un pago de la lista");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea borrar los pagos seleccionados?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var pago in gridPagos.SelectedItems.OfType<EmpeñosPago>().ToArray())
                    pagos.Remove(pago);

                ActualizarTotales();
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
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

            int saldo = txtTotalMontoPréstamo.AsInt - pagos.Where(p => p.Cuota <= gridPagos.SelectedIndex + 1).Sum(p => p.Abono);

            Guardar(false);
            Recibos.Imprimir("Imprimiento Recibo de Pago", Recibos.ReciboDePago(txtCódigo.AsInt.ToString(), cliente.Key, cliente.Value, gridPagos.SelectedIndex + 1, pago.FechaCuota, pago.FechaPago, pago.Intereses, pago.Abono, saldo, pago.Firma));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = this.DialogResult != true && MessageBox.Show("¿Está seguro que desea salir, sin guardar los cambios?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.No;
        }

        private void ActualizarTotales()
        {
            DateTime fechaÚltimoPago = dtpFecha.SelectedDate.Value;

            if (pagos.Count > 0)
            {
                txtTotalInteresesCancelados.AsInt = pagos.Sum(p => p.Intereses);

                int totalDePréstamoCancelado = pagos.Sum(p => p.Abono);

                txtTotalDePréstamoCancelado.AsInt = totalDePréstamoCancelado;

                int saldo = txtTotalMontoPréstamo.AsInt - totalDePréstamoCancelado;
                txtSaldoDelPréstamo.AsInt = saldo;
                btnNuevoPago.IsEnabled = btnRetirar.IsEnabled = saldo > 0;

                if (cmbEstado.SelectedIndex != (int)EstadosEmpeño.Quedado)
                    cmbEstado.SelectedIndex = saldo == 0 ? (int)EstadosEmpeño.Retirado : (int)EstadosEmpeño.Activo;

                fechaÚltimoPago = pagos.Max(p => p.FechaCuota);
                dtpFechaVencimiento.SelectedDate = fechaÚltimoPago.AddMonths(txtPlazo.AsInt);

                #region Formato Condicional

                if (txtTotalInteresesCancelados.AsInt > txtTotalMontoPréstamo.AsInt)
                {
                    txtTotalInteresesCancelados.Background = new SolidColorBrush(Colors.IndianRed);
                    txtTotalInteresesCancelados.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    txtTotalInteresesCancelados.Background = new SolidColorBrush(Colors.LightYellow);
                    txtTotalInteresesCancelados.Foreground = new SolidColorBrush(Colors.Black);

                }

                #endregion
            }
            else
            {
                txtTotalInteresesCancelados.AsInt = txtTotalDePréstamoCancelado.AsInt = 0;
                txtSaldoDelPréstamo.AsInt = txtTotalMontoPréstamo.AsInt;
                fechaÚltimoPago = dtpFecha.SelectedDate.Value;
                dtpFechaVencimiento.SelectedDate = fechaÚltimoPago.AddMonths(txtPlazo.AsInt);
            }
            double días = (DateTime.Today - fechaÚltimoPago).TotalDays;

            if (Convert.ToByte(cmbEstado.SelectedIndex) == (byte)EstadosEmpeño.Activo)
            {
                crtOrange.Maximum = (txtPlazo.AsInt * 30);
                crtYellow.Maximum = (txtPlazo.AsInt * 30) / 3 * 2;
                crtGreen.Maximum = (txtPlazo.AsInt * 30) / 3;

                if (días > (txtPlazo.AsInt * 30))
                {
                    if (ctrlIndicador.QualitativeRange.Count == 3)
                        ctrlIndicador.QualitativeRange.Add(new Controles.QualitativeRange { Color = Colors.Red, Maximum = días });

                    ctrlIndicador.Maximum = txtPlazo.AsInt;
                }
                else
                {
                    ctrlIndicador.Value = días;

                    if (ctrlIndicador.QualitativeRange.Count == 4)
                        ctrlIndicador.QualitativeRange.RemoveAt(3);

                    ctrlIndicador.Maximum = txtPlazo.AsInt * 30;
                }

                ctrlIndicador.Value = días;
                lblDías.Text = string.Format("{0} días pendientes", días);
            }
            else
            {
                ctrlIndicador.Visibility = System.Windows.Visibility.Collapsed;
                lblDías.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void dtpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarTotales();
        }

        private void GridPagos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridPagos.SelectedItem != null)
            {
                var pago = gridPagos.SelectedItem as EmpeñosPago;

                if (pago != null)
                {
                    var frmCuota = new FrmCuota(txtCódigo.AsInt, false, pago.FechaPago, pago.FechaCuota, pago.Intereses, pago.Abono, pago.Firma);

                    if (frmCuota.ShowDialog() == true)
                    {
                        pago.FechaPago = frmCuota.FechaPago;
                        pago.FechaCuota = frmCuota.FechaCuota;
                        pago.Intereses = frmCuota.Intereses;
                        pago.Abono = frmCuota.Abono;
                        pago.Firma = frmCuota.Firma;
                    }
                }
            }
        }

        private void BtnLimpiarFirma_Click(object sender, RoutedEventArgs e)
        {
            inkFirma.ClearTablet();
            inkFirma.SetTabletState(1);
        }
    }
}
