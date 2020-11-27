using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Promocion
    {
        public Int32 Prom_ID { get; set; }
        public  string Prom_Des { get; set; }
        public Decimal Prom_Porc { get; set; }
        public string Prom_FecIni { get; set; }
        public string Prom_FecFin { get; set; }
        public string Prom_EstID { get; set; }
        public Int32 Prom_Prioridad { get; set; }
        public string Prom_Tip_PromID { get; set; }
    }
    public class Ent_Promocion_Config
    {
        public string Prom_ID { get; set; }
        public string Prom_Tipo { get; set; }
        public string Prom_Des { get; set; }

    }
}
