using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Devolucion
{
    public class Ent_Documentos_Dev
    {
        public string TIPODOC { get; set; }
        public string NDOC { get; set; }
        public string FDOC { get; set; }
        public Decimal SUBTOTAL { get; set; }
        public Decimal IGV { get; set; }
        public Decimal TOTAL { get; set; }
    }
    public class Ent_Info_Devolucion
    {
        public List<Ent_Documentos_Dev> documentosDev { get; set; }
    }
    public class Ent_Documentos_Dev_Det_New
    {
        public string ARTICULO { get; set; }
        public string TALLA { get; set; }
        public Decimal CANTIDAD { get; set; }
    }
}
