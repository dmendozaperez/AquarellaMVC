using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Premios
    {
        public Int32 id { get; set; }
        public decimal monto { get; set; }
        public string descripcion { get; set; }
        public Int32 stock { get; set; }
        public Int32 stockingresado { get; set; }
    }
    public class Ent_Premios_Articulo
    {
        public string articulo { get; set; }
        public string talla { get; set; }
        public Decimal total { get; set; }
        public List<Ent_Articulo_Talla> list_talla { get; set; }
    }
    public class Ent_Lista_PremiosXArticulos
    {
        public Int32 id { get; set; }
        public string articulo { get; set; }
        public string talla { get; set; }
        public Int32 cantidad { get; set; }
        public Decimal precio { get; set; }
        public Int32 entregado { get; set; }
        public Int32 stock { get; set; }
    }
    public class Ent_Premios_Articulo_Stock
    {
        public string articulo { get; set; }
        public string talla { get; set; }
        public Int32 stock { get; set; }
        public Int32 cantidad { get; set; }
    }
}
