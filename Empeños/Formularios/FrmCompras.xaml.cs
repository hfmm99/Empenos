using Empeños.Clases;
using Empeños.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Empeños.Formularios
{
    /// <summary>
    /// Lógica de interacción para FrmCompras.xaml
    /// </summary>
    public partial class FrmCompras : Window
    {
        int? códigoCompra;
        bool modificandoArtículo = false;
        List<CaracterísticaValor> características;
        Parámetro parámetros;

        public FrmCompras()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        public FrmCompras(int pCódigoCompra)
            : this()
        {
            códigoCompra = pCódigoCompra;
        }

        public bool ImprimirAlGuardar
        {
            get { return chkImprimirAlGuardar.IsChecked == true; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            características = new List<CaracterísticaValor>();

            var bd = new EmpeñosDataContext();
            parámetros = bd.Parámetros.FirstOrDefault();

            cmbCategorías.Items.Insert(0, new KeyValuePair<int?, string>(null, string.Empty));

            foreach (var cat in bd.Categorías.OrderBy(cat => cat.Nombre))
                cmbCategorías.Items.Add(new KeyValuePair<int?, string>(cat.Código, cat.Nombre));

            cmbCategorías.SelectedIndex = 0;

            dtpFecha.SelectedDate = DateTime.Today;
            pnlDetalleArtículo.Visibility = Visibility.Collapsed;
            txtCódigo.Focus();

            if (códigoCompra.HasValue)
            {
                var compra = bd.Compras.SingleOrDefault(c => c.Código == códigoCompra);
                if (compra != null)
                {
                    txtCódigo.Text = compra.Código.ToString();
                    txtCódigo.IsReadOnly = true;
                    chkImprimirAlGuardar.Visibility = Visibility.Collapsed;

                    dtpFecha.SelectedDate = compra.Fecha;

                    if (compra.Cliente != null)
                    {
                        txtClientes.SelectedItem = new KeyValuePair<string, string>(compra.Cliente.Código, compra.Cliente.NombreCompleto);
                        txtClientes.Text = compra.Cliente.NombreCompleto;
                    }

                    txtTotalCostoCompra.AsInt = compra.CostoTotal;

                    foreach (ComprasDetalle det in compra.ComprasDetalles)
                    {
                        lstArtículos.Items.Add(det.Artículo);
                        det.Artículo.Artículos_Características.ToArray();
                    }

                    dtpFecha.Focus();
                }
            }
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
                    artículo = new Artículo { Estado = 1 };

                artículo.Nombre = txtNombre.Text;
                artículo.Costo = txtMontoCompra.AsInt;
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

                ActualizarTotalCostoCompra();

                Animaciones.DeslizarElementos(this, pnlListaArtículos, pnlDetalleArtículo);
                return true;
            }
            return false;
        }

        private void ActualizarTotalCostoCompra()
        {
            txtTotalCostoCompra.AsInt = (from Artículo art in lstArtículos.Items select (int?)art.Costo).Sum() ?? 0;
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
            if (txtMontoCompra.AsInt <= 0)
            {
                MessageBox.Show("Debe ingresar un monto de préstamo para el artículo, mayor a cero");
                return false;
            }
            return true;
        }

        private void LimpiarArtículo()
        {
            txtNombre.Clear();
            txtMontoCompra.Clear();
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
                txtMontoCompra.AsInt = artículo.Costo;
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
                ActualizarTotalCostoCompra();
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
            if (pnlDetalleArtículo.IsVisible)
                if (!GuardarArtículo())
                    return;

            if (ValidarCompra())
            {
                using (var bd = new EmpeñosDataContext())
                {
                    bool insertando = false;

                    var compra = bd.Compras.SingleOrDefault(em => em.Código == txtCódigo.AsInt);

                    if (compra == null)
                    {
                        compra = new Compra { Código = txtCódigo.AsInt };
                        bd.Compras.InsertOnSubmit(compra);
                        insertando = true;
                    }

                    compra.Código_Cliente = ((KeyValuePair<string, string>)txtClientes.SelectedItem).Key;
                    compra.Fecha = dtpFecha.SelectedDate.Value;
                    compra.CostoTotal = txtTotalCostoCompra.AsInt;
                    compra.Notas = txtNotas.Text;

                    if (compra.Estado != (byte)EstadosEmpeño.Quedado)
                    {
                        if (!insertando)
                        {
                            foreach (var detalle in compra.ComprasDetalles)
                            {
                                bd.Artículos_Características.DeleteAllOnSubmit(detalle.Artículo.Artículos_Características);
                                bd.Artículos.DeleteOnSubmit(detalle.Artículo);
                                bd.ComprasDetalles.DeleteOnSubmit(detalle);
                            }
                        }

                        for (int cont = 0; cont < lstArtículos.Items.Count; cont++)
                        {
                            var art = lstArtículos.Items[cont] as Artículo;

                            var nuevoArtículo = new Artículo
                                {
                                    Código = "C" + txtCódigo.AsInt.ToString() + "-" + (cont + 1).ToString(),
                                    Nombre = art.Nombre,
                                    Código_Categoría = art.Código_Categoría,
                                    Costo = art.Costo,
                                    Estado = art.Estado,
                                    Notas = art.Notas
                                };

                            foreach (var caract in art.Artículos_Características)
                                nuevoArtículo.Artículos_Características.Add(new Artículos_Característica { Código_Característica = caract.Código_Característica, Valor = caract.Valor });

                            compra.ComprasDetalles.Add(new ComprasDetalle { Artículo = nuevoArtículo });
                        }
                    }

                    bd.SubmitChanges();

                    this.DialogResult = true;
                }
            }
        }

        private bool ValidarCompra()
        {
            if (txtClientes.SelectedItem == null)
            {
                txtClientes.Text = string.Empty;
                MessageBox.Show("Debe seleccionar un cliente, o ingresar a la pantalla de clientes a insertar uno");
                txtClientes.Focus();
                return false;
            }

            if (txtTotalCostoCompra.AsInt == 0)
            {
                MessageBox.Show("Debe ingresar al menos un artículo");
                btnNuevoArtículo.Focus();
                return false;
            }

            if (dtpFecha.SelectedDate == null)
            {
                MessageBox.Show("Debe ingresar la fecha en que se realizó la compra");
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
            using (var bd = new EmpeñosDataContext())
            {
                if (!códigoCompra.HasValue)
                {
                    int últimoCódigo = bd.Compras.Select(comp => (int?)comp.Código).Max() ?? 0;
                    txtCódigo.AsInt = últimoCódigo + 1;
                    txtCódigo.SelectAll();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = this.DialogResult != true && MessageBox.Show("¿Está seguro que desea salir, sin guardar los cambios?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.No;
        }

        private void dtpFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
