using Empeños.Clases;
using Empeños.Datos;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Empeños.Formularios
{
    /// <summary>
    /// Lógica de interacción para FrmClientes.xaml
    /// </summary>
    public partial class FrmClientes : Window
    {
        #region Propiedades

        private String códigoCliente;

        public Cliente Cliente { get; private set; }

        #endregion

        public FrmClientes()
        {
            InitializeComponent();
        }

        public FrmClientes(string pCódigoCliente)
            : this()
        {
            códigoCliente = pCódigoCliente;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(códigoCliente))
            {
                using (var bd = new EmpeñosDataContext())
                {
                    var cliente = bd.Clientes.SingleOrDefault(c => c.Código == códigoCliente);
                    if (cliente != null)
                    {
                        txtCódigo.Text = cliente.Código;
                        txtCódigo.IsReadOnly = true;
    

                        txtNombre.Text = cliente.Nombre;
                        txtApellidos.Text = cliente.Apellidos;
                        rbMasculino.IsChecked = cliente.Género == 'M';
                        rbFemenino.IsChecked = cliente.Género == 'F';
                        txtTeléfono.Text = cliente.Teléfono;
                        txtEmail.Text = cliente.Email;
                        chkRecibirNotificaciones.IsChecked = cliente.RecibirNotificaciones;
                        txtDirección.Text = cliente.Dirección;
                        txtNotas.Text = cliente.Notas;

                        if (cliente.Foto != null)
                        {
                            var bitmap = new BitmapImage();

                            using (var stream = new MemoryStream(cliente.Foto.ToArray()))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                            }
                            imgFoto.Source = bitmap;
                        }

                        txtNombre.Focus();
                    }
                }
            }
            else
                txtCódigo.Focus();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (txtCódigo.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debe ingresar la identificación del cliente");
                txtCódigo.Focus();
                return;
            }

            if (txtNombre.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debe ingresar el nombre del cliente");
                txtNombre.Focus();
                return;
            }

            if (txtApellidos.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debe ingresar los apellidos del cliente");
                txtApellidos.Focus();
                return;
            }

            string caseSwitch = cmbTipoId.Text;
            int tipoid;
            switch (caseSwitch)
            {
                case "Física":
                    tipoid = 1;
                    break;
                case "Jurídica":
                    tipoid = 2;
                    break;
                case "NITE":
                    tipoid = 3;
                    break;
                case "DIMEX":
                    tipoid = 4;
                    break;
                default:
                    tipoid = 5;
                    break;
            }

            using (var bd = new EmpeñosDataContext())
            {
                Cliente = bd.Clientes.SingleOrDefault(c => c.Código == txtCódigo.Text.Trim());

                if (Cliente == null)
                {
                    Cliente = new Cliente();
                    bd.Clientes.InsertOnSubmit(Cliente);
                }

                Cliente.Código = txtCódigo.Text.Trim();
                Cliente.Nombre = txtNombre.Text.Trim();
                Cliente.Apellidos = txtApellidos.Text.Trim();
                Cliente.Género = rbMasculino.IsChecked.Value ? 'M' : 'F';
                Cliente.Teléfono = txtTeléfono.Text;
                Cliente.Email = txtEmail.Text;
                Cliente.RecibirNotificaciones = chkRecibirNotificaciones.IsChecked.Value;
                Cliente.Dirección = txtDirección.Text;
                Cliente.Notas = txtNotas.Text;
                Cliente.TipoCedula = tipoid;

                if (imgFoto.Source != null && !imgFoto.Source.ToString().StartsWith("pack"))
                {
                    using (var stream = new MemoryStream())
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imgFoto.Source as BitmapSource));
                        encoder.Save(stream);
                        Cliente.Foto = stream.ToArray();
                    }
                }
                else
                    Cliente.Foto = null;

                this.DialogResult = true;

                bd.SubmitChanges();
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnEscanearFoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var imagen = new ScannerService().Scan();

                if (imagen != null)
                    imgFoto.Source = new ScannerImageConverter().InMemoryConvertScannedImage(imagen);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al escanear foto, asegúrese que el escaner está encendido:\n" + ex.Message);
            }
        }

        private void BtnTomarFoto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("No implementado aún");
        }

        private void BtnSeleccionarFoto_Click(object sender, RoutedEventArgs e)
        {
            var dglAbrirArchivo = new OpenFileDialog { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png" };

            if (dglAbrirArchivo.ShowDialog() == true)
            {
                imgFoto.Source = new BitmapImage(new Uri(dglAbrirArchivo.FileName));
            }
        }

        private void BtnBorrarFoto_Click(object sender, RoutedEventArgs e)
        {
            imgFoto.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Recursos/Imágenes/Logo.png"));
        }
    }
}
