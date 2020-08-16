using Empeños.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Empeños.Formularios
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class FrmActualizarID : Window
    {
        public FrmActualizarID(string id, string tipo)
        {
            InitializeComponent();
            txtID.Text = id;
            txtTipoID.Text = tipo;
            //cmbActualizarTipoID.Text = tipo;
        }

        private void btnActualizar_click(object sender, RoutedEventArgs e)
        {
            if (txtActualizarID.Value == null)
            {
                MessageBox.Show("Debe digitar la nueva identificación");
                return;
            }
            if (cmbActualizarTipoID.Text == "")
            {
                MessageBox.Show("Debe digitar el nuevo tipo de identificación");
                return;
            }
            if (MessageBox.Show("¿Está seguro que desea actualizar los datos?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                DialogResult = true;
        }


        private void btnCancelar_click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desea salir sin guardar los cambios?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                DialogResult = false;
        }

        private void ChangeMaskID(object sender, RoutedEventArgs e)
        {

            if (txtActualizarID != null)
            {
                txtActualizarID.Clear();
                switch (cmbActualizarTipoID.SelectedIndex)
                {
                    case 0:
                        txtActualizarID.Mask = "0 0000 0000";
                        txtActualizarID.IncludeLiteralsInValue = false;
                        break;
                    case 1:
                        txtActualizarID.Mask = "0 000 000000";
                        break;
                    case 2:
                        txtActualizarID.Mask = "000000000000";
                        break;
                    case 3:
                        txtActualizarID.Mask = "0 000 000000";
                        break;
                    default:
                        txtActualizarID.Mask = "";
                        break;
                }
                txtActualizarID.Focus();
            }
        }
    }
}
