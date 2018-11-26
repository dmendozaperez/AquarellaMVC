using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.ValeCompra
{
    public class Ent_ValeCompraDetalle
    {
        public Int32 valCompra_id { get; set; }
        public Int32 valComDet_id { get; set; }
        public Int32 valComDet_cantidad { get; set; }

        public string valComDet_correlativo { get; set; }
        public Decimal valComDet_monto { get; set; }
        public string valComDet_montoLetras { get; set; }
        public string valCom_usuCrea { get; set; }
        public string valCom_codeBarra { get; set; }
        public string valCom_fecCrea { get; set; }
        public string valCom_IpCrea { get; set; }

    }
}
