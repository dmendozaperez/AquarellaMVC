using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Articulo_Stock
    {
        public string id_almacen { get; set; }
        public string almacen { get; set; }
        public string articulo { get; set; }
        public Decimal total { get; set; }
        public List<Ent_Articulo_Talla> list_talla { get; set; }
        public List<Ent_Pedido_Sep> list_pedido_sep { get; set; }
    }
    public class Ent_Articulo_Talla
    {
        public string talla { get; set; }
        public Int32 cantidad { get; set; }
    }
    public class Ent_Pedido_Sep
    {
        public string talla { get; set; }
        public string ped_sep { get; set; }
    }
    public class Ent_Articulo_Info
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public string marca { get; set; }
        public string linea { get; set; }
        public string color { get; set; }
        public string temporada { get; set; }
        public decimal precio { get; set; }
        public decimal costo { get; set; }
        public string foto { get; set; }
    }

    public class Ent_Articulo_Sin_Stock
    {
        public string pedido { get; set; }
        public string dni { get; set; }
        public string nombres { get; set; }
        public string articulo { get; set; }
        public string talla { get; set; }
        public string estado { get; set; }

    }

}
