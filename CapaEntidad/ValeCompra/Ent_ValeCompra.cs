using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.ValeCompra
{
    public class Ent_ValeCompra
    {    
       
        public Int32 valCompra_id { get; set; }
        public Int32 valCliente_id { get; set; }
        public string valCompra_ruc { get; set; }
        public string valCompra_razon { get; set; }
        public string valCompra_descripcion { get; set; }
        public string valCompra_fecVigenIni { get; set; }
        public string valCompra_fecVigenFin { get; set; }
        public string valCompra_usuCreacion { get; set; }
        public string valCompra_fecCreacion { get; set; }
        public string valCompra_ipCreacion { get; set; }
        public string valCompra_generado { get; set; }
        public string valCompra_total { get; set; }
        public string valCompra_strListDetalle { get; set; }
        public List<Ent_ValeCompraDetalle> valCompra_ListDetalle { get; set; }

        public string cli_codigo { get; set; }
        public string cli_Direccion { get; set; }



    }
    
}
