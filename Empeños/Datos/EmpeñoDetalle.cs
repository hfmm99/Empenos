//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Empeños.Datos
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmpeñoDetalle
    {
        public int Código_Empeño { get; set; }
        public string Código_Artículo { get; set; }
        public decimal Precio { get; set; }
    
        public virtual Artículo Artículos { get; set; }
        public virtual Empeño Empeños { get; set; }
    }
}
