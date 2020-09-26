using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Pedido
{
    public class Ent_Pedido_Separado
    {
        public string asesor { get; set; }
        public string lider { get; set; }
        public string pedido { get; set; }
        public string promotor { get; set; }
        public string fecha_ing { get; set; }
        public string fecha_cad { get; set; }
        public Int32 tcantidad { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public string ubicacion { get; set; }      
        public Int32 dias_pedido { get; set; }
        public decimal subtotal { get; set; }

    }
}
