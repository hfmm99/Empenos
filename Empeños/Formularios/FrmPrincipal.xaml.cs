using Empeños.Clases;
using Empeños.Datos;
using Empeños.Reportes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Xceed.Wpf.DataGrid;

namespace Empeños.Formularios
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class FrmPrincipal : Window
    {
        #region Métodos

        public FrmPrincipal()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        private void Modificar()
        {
            switch (tabOpciones.SelectedIndex)
            {
                case 0: //Empeños
                    if (dgEmpeños.SelectedItem != null)
                        new FrmEmpeños((dgEmpeños.SelectedItem as Empeño).Código) { Owner = this }.ShowDialog();
                    break;
                case 1: //Compras
                    if (dgCompras.SelectedItem != null)
                        new FrmCompras((dgCompras.SelectedItem as Compra).Código) { Owner = this }.ShowDialog();
                    break;
                case 2: //Ventas
                    if (dgVentas.SelectedItem != null)
                        new FrmVentas((dgVentas.SelectedItem as Venta).Código) { Owner = this }.ShowDialog();
                    break;
                case 3: //Clientes
                    if (dgClientes.SelectedItem != null)
                        new FrmClientes((dgClientes.SelectedItem as Cliente).Código) { Owner = this }.ShowDialog();
                    break;
                case 4: //Reportes
                    if (lstReportes.SelectedItem != null)
                        Process.Start(new ProcessStartInfo(((KeyValuePair<string, string>)lstReportes.SelectedItem).Value) { UseShellExecute = true });
                    break;
                default:
                    break;
            }
        }

        private void Buscar()
        {
            var bd = new EmpeñosDataContext();

            switch (tabOpciones.SelectedIndex)
            {
                case 0: //Empeños
                    var empeños = bd.Empeños.AsQueryable();

                    if (txtBuscar.Text.Trim() == string.Empty)
                    {
                        empeños = empeños.Where(em => em.Fecha.Date == DateTime.Today);
                    }
                    else
                    {
                        empeños = empeños.Where(em => em.Estado != (byte)EstadosEmpeño.Inactivo && (em.Código.ToString().Contains(txtBuscar.Text) || em.Cliente.Código.Contains(txtBuscar.Text) || em.Cliente.NombreCompleto.Contains(txtBuscar.Text)));
                    }

                    dgEmpeños.ItemsSource = empeños
                                            .OrderBy(em => em.Estado)
                                            .ThenByDescending(em => em.Fecha)
                                            .ToList();

                    break;

                case 1: //Compras
                    var compras = bd.Compras.AsQueryable();

                    if (txtBuscar.Text.Trim() == string.Empty)
                    {
                        compras = compras.Where(em => em.Fecha.Date == DateTime.Today);
                    }
                    else
                    {
                        compras = compras.Where(comp => comp.Código.ToString().Contains(txtBuscar.Text) || comp.Cliente.Código.Contains(txtBuscar.Text) || comp.Cliente.NombreCompleto.Contains(txtBuscar.Text));
                    }

                    dgCompras.ItemsSource = compras
                                           .OrderBy(em => em.Estado)
                                           .ThenByDescending(em => em.Fecha)
                                           .ToList();
                    break;

                case 2://Ventas
                    var ventas = bd.Ventas.AsQueryable();

                    if (txtBuscar.Text.Trim() == string.Empty)
                    {
                        ventas = ventas.Where(em => em.Fecha.Date == DateTime.Today);
                    }
                    else
                    {
                        ventas = ventas.Where(vent => vent.Código.ToString().Contains(txtBuscar.Text) || vent.Cliente.Código.Contains(txtBuscar.Text) || vent.Cliente.NombreCompleto.Contains(txtBuscar.Text));
                    }

                    dgVentas.ItemsSource = ventas
                                          .OrderBy(em => em.Estado)
                                          .ThenByDescending(em => em.Fecha)
                                          .ToList();
                    break;

                default:
                    if (txtBuscar.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("Debe digitar al menos un caracter para realizar una búsqueda");
                    }
                    else
                    {
                        dgClientes.ItemsSource = bd.Clientes
                                                   .Where(c => c.Código.Contains(txtBuscar.Text) || c.NombreCompleto.Contains(txtBuscar.Text))
                                                   .OrderBy(c => c.NombreCompleto)
                                                   .ToList();
                    }
                    break;
            }
        }

        private void AgruparGrid(DataGridControl dataGrid, params string[] propertyNames)
        {
            ICollectionView cvGrid = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            if (cvGrid != null)
            {
                if (cvGrid.CanGroup)
                {
                    cvGrid.GroupDescriptions.Clear();

                    foreach (string property in propertyNames)
                        cvGrid.GroupDescriptions.Add(new PropertyGroupDescription(property));
                }

                if (cvGrid.CanSort)
                {
                    cvGrid.SortDescriptions.Clear();

                    foreach (string property in propertyNames)
                        cvGrid.SortDescriptions.Add(new SortDescription(property, ListSortDirection.Ascending));
                }
            }
        }

        #endregion

        #region Eventos

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Buscar();
        }

        private void dgEmpeños_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Modificar();
        }

        private void dgEmpeños_SelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            var empeño = dgEmpeños.SelectedItem as Empeño;

            if (empeño != null)
            {
                btnReempeñar.Visibility = empeño != null && empeño.Estado == (byte)EstadosEmpeño.Retirado ? Visibility.Visible : Visibility.Hidden;

                if (empeño.Estado == (byte)EstadosEmpeño.Quedado)
                    btnQuedado.Content = "Reactivar";
                else
                    btnQuedado.Content = "Quedado";
            }
        }

        private void dgEmpeños_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Modificar();
        }

        private void dgEmpeños_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            AgruparGrid(dgEmpeños, "EstadoEmpeño");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBuscar.Focus();

            foreach (string item in Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Reportes\\"), "*.rdl"))
                lstReportes.Items.Add(new KeyValuePair<string, string>(System.IO.Path.GetFileNameWithoutExtension(item), item));

            using (var bd = new EmpeñosDataContext())
            {
                var parámetros = bd.Parámetros.FirstOrDefault();

                if (parámetros != null)
                    webBrowser.Source = new Uri(parámetros.RutaServidorReportes);
            }
        }

        private void txtBuscar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Buscar();
        }

        private void tabOpciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Title = "La salvada - " + (tabOpciones.SelectedItem as TabItem).Header.ToString();
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            switch (tabOpciones.SelectedIndex)
            {
                case 0: //Empeños
                    var frmEmpeños = new FrmEmpeños() { Owner = this };
                    frmEmpeños.ShowDialog();

                    if (frmEmpeños.ImprimirAlGuardar)
                        if (frmEmpeños.txtClientes.Text != "")
                        {

                            Recibos.Imprimir("Imprimiendo Recibo", Recibos.ReciboDeEmpeño(frmEmpeños.txtCódigo.AsInt, frmEmpeños.inkFirma.GetSigString()));
                        }
                    break;
                case 1: //Compras
                    var frmCompras = new FrmCompras() { Owner = this };
                    frmCompras.ShowDialog();

                    if (frmCompras.ImprimirAlGuardar)
                        Recibos.Imprimir("Imprimiendo Recibo", Recibos.ReciboDeCompra(frmCompras.txtCódigo.AsInt));
                    break;
                case 2: //Ventas
                    var frmVentas = new FrmVentas() { Owner = this };
                    frmVentas.ShowDialog();

                    if (frmVentas.ImprimirAlGuardar)
                        Recibos.Imprimir("Imprimiendo Recibo", Recibos.ReciboDeVenta(frmVentas.txtCódigo.AsInt));
                    break;
                case 3: //Clientes
                    new FrmClientes() { Owner = this }.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            Modificar();
        }

        private void btnImprimirEmpeño_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmpeños.SelectedItem != null)
                Recibos.Imprimir("Imprimiendo Recibo", Recibos.ReciboDeEmpeño((dgEmpeños.SelectedItem as Empeño).Código, (dgEmpeños.SelectedItem as Empeño).Firma));
        }

        private void btnReempeñar_Click(object sender, RoutedEventArgs e)
        {
            var empeño = dgEmpeños.SelectedItem as Empeño;

            if (empeño != null)
                new FrmEmpeños(empeño.Código, true) { Owner = this }.ShowDialog();
        }

        private void btnAgregarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnAgregar_Click(sender, e);
        }

        private void btnEditarCliente_Click(object sender, RoutedEventArgs e)
        {
            Modificar();
        }

        private void btnEliminarEmpeño_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmpeños.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un empeño de la lista");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea borrar el empeño seleccionado?\nEsto eliminará del sistema todos los artículos ingresados en este empeño, sus pagos, y el empeño en sí; pero conservará el registro del cliente.", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    var empeño = bd.Empeños.FirstOrDefault(emp => emp.Código == (dgEmpeños.SelectedItem as Empeño).Código);

                    if (empeño != null)
                    {
                        try
                        {
                            if (empeño.EmpeñosDetalles.Any(cd => cd.Artículo.Estado != (int)EstadosActículos.Normal))
                            {
                                MessageBox.Show("Este empeño no se puede eliminar porque algunos de sus artículos ya fueron vendidos o apartados");
                                return;
                            }

                            foreach (var detalle in empeño.EmpeñosDetalles)
                                detalle.Artículo.Estado = (byte)EstadosActículos.Inactivo;

                            empeño.Estado = (byte)EstadosEmpeño.Inactivo;
                            bd.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se produjo un error al intentar eliminar el empeño seleccionado, por favor asegurese que los artículos no estén registrados en facturas de ventas, antes de intentar eliminarlos.\n\n" + ex.Message);
                        }
                        Buscar();
                    }
                }
            }
        }

        private void btnImprimirCompra_Click(object sender, RoutedEventArgs e)
        {
            if (dgCompras.SelectedItem != null)
                Recibos.Imprimir("Imprimiendo Recibo", Recibos.ReciboDeCompra((dgCompras.SelectedItem as Compra).Código));
        }

        private void btnAbrirReporte_Click(object sender, RoutedEventArgs e)
        {
            Modificar();
        }

        private void btnQuedado_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmpeños.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un empeño de la lista");
                return;
            }

            string pregunta = "¿Está seguro que desea cambiar el estado del empeño seleccionado a " +
                               ((sender as Button).Content.ToString() == "Quedado" ?
                               "'Quedado'? \n Esto pondrá los artículos del empeño disponibles para la venta, y prevendrá que pueda volver a realizar pagos al mismo." :
                               "'Activo'?");

            if (MessageBox.Show(pregunta, "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    var empeño = bd.Empeños.FirstOrDefault(emp => emp.Código == (dgEmpeños.SelectedItem as Empeño).Código);

                    if (empeño != null)
                    {
                        try
                        {
                            empeño.Estado = empeño.Estado == (byte)EstadosEmpeño.Quedado ? (byte)EstadosEmpeño.Activo : (byte)EstadosEmpeño.Quedado;

                            foreach (var detalle in empeño.EmpeñosDetalles)
                                detalle.Artículo.Estado = empeño.Estado == (byte)EstadosEmpeño.Quedado ? (byte)EstadosActículos.Quedado : (byte)EstadosActículos.Normal;

                            bd.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se produjo un error al intentar cambiar el estado del empeño a 'Quedado'.\n\n" + ex.Message);
                        }
                        Buscar();
                    }
                }
            }
        }

        private void btnAgregarVenta_Click(object sender, RoutedEventArgs e)
        {
            new FrmVentas().ShowDialog();
        }

        private void btnEliminarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (dgVentas.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una venta de la lista");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea borrar la factura de venta seleccionada?\nEsto pondrá de vuelta a todos sus artículos disponibles para la venta.", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    var venta = bd.Ventas.FirstOrDefault(v => v.Código == (dgVentas.SelectedItem as Venta).Código);

                    if (venta != null)
                    {
                        try
                        {
                            //Cambiar estado a artículos
                            foreach (var detalle in venta.VentasDetalles)
                                detalle.Artículo.Estado = (byte)EstadosActículos.Quedado;

                            venta.Estado = (byte)EstadosEmpeño.Inactivo;
                            bd.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se produjo un error al intentar eliminar la factura seleccionada\n" + ex.Message);
                        }
                        Buscar();
                    }
                }
            }
        }

        private void btnEliminarCompra_Click(object sender, RoutedEventArgs e)
        {
            if (dgCompras.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una compra de la lista");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea borrar la compra seleccionada?\nEsto eliminará del sistema todos los artículos ingresados en esta compra; pero conservará el registro del cliente.", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    var compra = bd.Compras.FirstOrDefault(c => c.Código == (dgCompras.SelectedItem as Compra).Código);

                    if (compra != null)
                    {
                        if (compra.ComprasDetalles.Any(cd => cd.Artículo.Estado != (int)EstadosActículos.Normal))
                        {
                            MessageBox.Show("Esta compra no se puede eliminar porque algunos de sus artículos ya fueron vendidos");
                            return;
                        }

                        try
                        {
                            foreach (var detalle in compra.ComprasDetalles)
                                detalle.Artículo.Estado = (byte)EstadosActículos.Inactivo;

                            compra.Estado = (byte)EstadosEmpeño.Inactivo;
                            bd.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se produjo un error al intentar eliminar la compra seleccionada, por favor asegurese que los artículos no estén registrados en facturas de ventas, antes de intentar eliminarlos.\n\n" + ex.Message);
                        }
                        Buscar();
                    }
                }
            }
        }

        private void btnImprimirVenta_Click(object sender, RoutedEventArgs e)
        {
            if (dgVentas.SelectedItem != null)
                Recibos.Imprimir("Imprimiendo Recibo", Recibos.ReciboDeVenta((dgVentas.SelectedItem as Venta).Código));
        }

        #endregion

        private void btnEliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una venta de la lista");
                return;
            }

            if (MessageBox.Show("¿Está seguro que desea borrar el cliente seleccionado?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var bd = new EmpeñosDataContext())
                {
                    var cliente = bd.Clientes.FirstOrDefault(c => c.Código == (dgClientes.SelectedItem as Cliente).Código);

                    if (cliente != null)
                    {
                        try
                        {

                            bd.Clientes.DeleteOnSubmit(cliente);
                            bd.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se produjo un error al intentar eliminar el cliente, tiene otros registros asociados\n" + ex.Message);
                        }
                        Buscar();
                    }
                }
            }
        }
    }
}
