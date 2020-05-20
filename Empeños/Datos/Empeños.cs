using System.Text;
using System.Linq;
using Empe�os.Clases;

namespace Empe�os.Datos
{
    partial class Art�culo
    {
        public override string ToString()
        {
            string nombre = Descripci�n;

            if (!string.IsNullOrEmpty(Notas))
                nombre += "\n   Nota: " + Notas;

            return nombre;
        }

        public string Descripci�n
        {
            get
            {
                var nombre = new StringBuilder(Nombre);

                foreach (var car in this.Art�culos_Caracter�sticas.Where(car => !string.IsNullOrEmpty(car.Valor)))
                    nombre.Append(". " + car.Valor);

                return nombre.ToString();
            }
        }

        public string Descripci�nExtendida
        {
            get
            {
                var nombre = new StringBuilder(Nombre);

                foreach (var car in this.Art�culos_Caracter�sticas.Where(car => !string.IsNullOrEmpty(car.Valor)))
                    nombre.Append($".  {car.Caracter�stica.Nombre}: {car.Valor}");

                return nombre.ToString();
            }
        }
    }

    partial class Empe�o
    {
        public EstadosEmpe�o EstadoEmpe�o
        {
            get
            {
                return (EstadosEmpe�o)this.Estado;
            }
        }
    }

    partial class Venta
    {
        public EstadosEmpe�o EstadoVenta
        {
            get { return (EstadosEmpe�o)this.Estado; }
        }
    }

    partial class Compra
    {
        public EstadosEmpe�o EstadoCompra
        {
            get { return (EstadosEmpe�o)this.Estado; }
        }
    }
}
