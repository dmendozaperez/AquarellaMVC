using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Reports
{
    public class Ent_Bataclub
    {
        public string cod_tienda { get; set; }
        public string des_tienda { get; set; }
        public string semana { get; set; }
        public String fecha { get; set; }
        public string dni { get; set; }
        public string bolfac { get; set; }
        public Decimal soles { get; set; }
        public Int32 pares { get; set; }

        public string estado { get; set; }

        public string fecha_ing { get; set; }
        public string promocion { get; set; }
    }
}
