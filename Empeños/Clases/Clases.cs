using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Threading;

namespace Empeños.Clases
{
    public class CaracterísticaValor
    {
        public int CódigoCaracterística { get; set; }
        public string NombreCaracterística { get; set; }
        public string Valor { get; set; }
    }

    public enum EstadosEmpeño
    {
        Activo,
        Retirado,
        Quedado,
        Inactivo
    }

    public enum EstadosActículos
    {
        Normal,
        Quedado,
        Devuelto,
        Extraviado,
        Inactivo,
        Vendido
    }

    public class RowToIndexConv : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class EasyTimer
    {
        public static IDisposable SetInterval(Action method, int delayInMilliseconds)
        {
            System.Timers.Timer timer = new System.Timers.Timer(delayInMilliseconds);
            timer.Elapsed += (source, e) =>
            {
                method();
            };

            timer.Enabled = true;
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }

        public static IDisposable SetTimeout(Action method, int delayInMilliseconds)
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(delayInMilliseconds);
            timer.Tick += (source, e) =>
            {
                method();
                timer.Stop();
            };
            timer.Start();

            // Returns a stop handle which can be used for stopping
            // the timer, if required
            return timer as IDisposable;
        }
    }
}
