using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Ent_Pedido
    {
        public string liq_PedId  { get; set; }
        public string liq_Fecha { get; set; }
        public decimal Pares { get; set; } 
        public string Estado { get; set; }
        public string Ganancia { get; set; }
        public string Subtotal { get; set; }
        public string N_C { get; set; }
        public string Total { get; set; }
        public string Percepcion { get; set; }
        public decimal TotalPagar { get; set; }

    }    
}
