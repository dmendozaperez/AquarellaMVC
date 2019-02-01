using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Ent_Pago_NCredito
    {
        public bool Consumido { get; set; }
        public bool Activado { get; set; }
        public string Ncredito { get; set; }
        public decimal Importe { get; set; } 
        public string Rhv_return_nro { get; set; }
        public DateTime Fecha_documento { get; set; }       

    }    
}
