using System.Text;
using System.Linq;
using Empeños.Clases;

namespace Empeños.Datos
{
    partial class Artículo
    {
        public override string ToString()
        {
            string nombre = Descripción;

            if (!string.IsNullOrEmpty(Notas))
                nombre += "\n   Nota: " + Notas;

            return nombre;
        }

        public string Descripción
        {
            get
            {
                var nombre = new StringBuilder(Nombre);

                foreach (var car in this.Artículos_Características.Where(car => !string.IsNullOrEmpty(car.Valor)))
                    nombre.Append(". " + car.Valor);

                return nombre.ToString();
            }
        }

        public string DescripciónExtendida
        {
            get
            {
                var nombre = new StringBuilder(Nombre);

                foreach (var car in this.Artículos_Características.Where(car => !string.IsNullOrEmpty(car.Valor)))
                    nombre.Append($".  {car.Característica.Nombre}: {car.Valor}");

                return nombre.ToString();
            }
        }
    }

    partial class Empeño
    {
        public EstadosEmpeño EstadoEmpeño
        {
            get
            {
                return (EstadosEmpeño)this.Estado;
            }
        }
    }

    partial class Venta
    {
        public EstadosEmpeño EstadoVenta
        {
            get { return (EstadosEmpeño)this.Estado; }
        }
    }

    partial class Compra
    {
        public EstadosEmpeño EstadoCompra
        {
            get { return (EstadosEmpeño)this.Estado; }
        }
    }
}
