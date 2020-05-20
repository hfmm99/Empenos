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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Empeños.Controles
{
    /// <summary>
    /// Lógica de interacción para IndicadorEstado.xaml
    /// </summary>
    public partial class IndicadorEstado : UserControl
    {
        EstadoIndicador estado;

        public IndicadorEstado()
        {
            InitializeComponent();
            Estado = EstadoIndicador.Verde;
        }

        public EstadoIndicador Estado
        {
            get { return estado; }
            set
            {
                estado = value;

                switch (value)
                {
                    case EstadoIndicador.Verde:
                        pathVerde.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00"));
                        pathAmarillo.Fill = new SolidColorBrush(Colors.Transparent);
                        pathRojo.Fill = new SolidColorBrush(Colors.Transparent);
                        break;
                    case EstadoIndicador.Amarillo:
                        pathVerde.Fill = new SolidColorBrush(Colors.Transparent);
                        pathAmarillo.Fill = new SolidColorBrush(Colors.Yellow);
                        pathRojo.Fill = new SolidColorBrush(Colors.Transparent);
                        break;
                    case EstadoIndicador.Rojo:
                        pathVerde.Fill = new SolidColorBrush(Colors.Transparent);
                        pathAmarillo.Fill = new SolidColorBrush(Colors.Transparent);
                        pathRojo.Fill = new SolidColorBrush(Colors.Red);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public enum EstadoIndicador
    {
        Verde,
        Amarillo,
        Rojo
    }
}
