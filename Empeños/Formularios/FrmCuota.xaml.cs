using System;
using System.Windows;

namespace Empeños.Formularios
{
    public partial class FrmCuota : Window
    {
        #region Propiedades

        public DateTime FechaPago => dtpFechaPago.Value.Value;

        public DateTime FechaCuota => dtpFechaCuota.SelectedDate.Value;

        public int Intereses => txtIntereses.AsInt;

        public int Abono => txtAbono.AsInt;

        public string Firma
        {
            get
            {
                if (inkFirma.NumberOfTabletPoints() == 0)
                    return null;

                return inkFirma.GetSigString();
            }
        }

        #endregion

        #region Métodos

        public FrmCuota(int códigoEmpeño, bool retiro, DateTime fechaPago, DateTime fechaCuota, int intereses, int abono, string firma = null)
        {
            InitializeComponent();

            this.Title = (retiro ? "Retirar" : "Cancelar una cuota de") + " empeño: " + códigoEmpeño;

            dtpFechaPago.Value = fechaPago;
            dtpFechaCuota.SelectedDate = fechaCuota;
            txtIntereses.AsInt = intereses;
            txtAbono.AsInt = abono;

            // Encriptar y Comprimir Firma
            inkFirma.SetSigCompressionMode(2);
            inkFirma.SetEncryptionMode(2);
            inkFirma.AutoKeyStart();
            inkFirma.SetAutoKeyData("La Salvada");
            inkFirma.AutoKeyFinish();

            if (firma != null && firma.Length > 0)
                inkFirma.SetSigString(firma);

            inkFirma.SetTabletState(1);
        }

        private void BtnLimpiarFirma_Click(object sender, RoutedEventArgs e)
        {
            inkFirma.ClearTablet();
            inkFirma.SetTabletState(1);
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (dtpFechaPago.Value == null)
            {
                MessageBox.Show("Debe digitar la fecha de Pago");
                return;
            }

            if (dtpFechaCuota.SelectedDate == null)
            {
                MessageBox.Show("Debe digitar la fecha de la Cuota");
                return;
            }

            if (txtIntereses.AsInt <= 0 && txtAbono.AsInt <= 0)
            {
                MessageBox.Show("Debe ingresar ya sea Intereses o Monto de Abono");
                return;
            }

            if (inkFirma.NumberOfTabletPoints() == 0)
                DialogResult = MessageBox.Show("¿No ha firmado el Pago, está seguro que desea salir?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            else
                DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desea salir, sin guardar los cambios?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                DialogResult = false;
        }

        #endregion
    }
}
