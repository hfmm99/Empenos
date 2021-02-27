using System;
using System.Windows;

namespace Empeños.Formularios
{
    public partial class FrmAbono : Window
    {
        #region Propiedades

        public DateTime Fecha => dtpFechaAbono.Value.Value;

        public int Monto => txtAbono.AsInt;

        /*public string Firma
        {
            get
            {
                if (inkFirma.NumberOfTabletPoints() == 0)
                    return null;

                return inkFirma.GetSigString();
            }
        }*/

        #endregion

        #region Métodos

        public FrmAbono(int códigoVenta, int cuota, DateTime fechaAbono, int abono)
        {
            InitializeComponent();

            this.Title = ("Cancelar una cuota del") + " apartado: " + códigoVenta;

            dtpFechaAbono.Value = fechaAbono;
            txtAbono.AsInt = abono;

            // Encriptar y Comprimir Firma
           /* inkFirma.SetSigCompressionMode(2);
            inkFirma.SetEncryptionMode(2);
            inkFirma.AutoKeyStart();
            inkFirma.SetAutoKeyData("La Salvada");
            inkFirma.AutoKeyFinish();

            if (firma != null && firma.Length > 0)
                inkFirma.SetSigString(firma);

            inkFirma.SetTabletState(1);*/
        }

        private void BtnLimpiarFirma_Click(object sender, RoutedEventArgs e)
        {
          //  inkFirma.ClearTablet();
           // inkFirma.SetTabletState(1);
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            /*  if (dtpFechaPago.Value == null)
              {
                  MessageBox.Show("Debe digitar la fecha de Pago");
                  return;
              }

              if (dtpFechaCuota.SelectedDate == null)
              {
                  MessageBox.Show("Debe digitar la fecha de la Cuota");
                  return;
              }*/

            if (txtAbono.AsInt <= 0)
            {
                MessageBox.Show("Debe ingresar el monto del abono");
                return;
            }
            /*

              if (inkFirma.NumberOfTabletPoints() == 0)
                  DialogResult = MessageBox.Show("¿No ha firmado el Pago, está seguro que desea salir?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes; ;*/
            else
                DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
           /* if (MessageBox.Show("¿Está seguro que desea salir, sin guardar los cambios?", "Pregunta", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                DialogResult = false;*/
        }

        #endregion
    }
}
