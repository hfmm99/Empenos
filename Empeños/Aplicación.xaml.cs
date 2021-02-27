using Empeños.Datos;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Linq;
using System.IO;

namespace Empeños
{
    public partial class Aplicación : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var ci = new CultureInfo("es-CR");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                using (var bd = new EmpeñosDataContext())
                {                    
                    var parametros = bd.Parámetros.FirstOrDefault();

                    if (parametros != null)
                    {
                        string rutaRespaldo = Path.GetDirectoryName(parametros.RutaRespaldoBD);
                        string rutaRespaldosViejos = Path.Combine(rutaRespaldo, "Viejos\\");
                        string archivoRespaldo = Path.Combine(rutaRespaldo, "Empeños.bak");

                        try 
                    	{	        
                            if(!Directory.Exists(rutaRespaldosViejos))
                                Directory.CreateDirectory(rutaRespaldosViejos);

                            string archivoRespaldoViejo = Path.Combine(rutaRespaldosViejos, "Empeños_" + DateTime.Today.DayOfWeek + ".bak");

                            if(File.Exists(archivoRespaldoViejo))
                                File.Delete(archivoRespaldoViejo);

                            File.Move(archivoRespaldo, archivoRespaldoViejo);
                        }
	                    catch
                        { }

                        bd.Respaldar(archivoRespaldo);
                    }
                }
            }
            catch (Exception ex) { ex.ToString(); }
        }
    }
}
