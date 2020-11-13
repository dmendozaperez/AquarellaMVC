using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Articulo_Precio
    {
        public string tipo { get; set; }
        public string tipodes { get; set; }
        public string articulo { get; set; }
        public string descripcion { get; set; }
        public Decimal precioigv { get; set; }
        public Decimal precion { get; set; }
        public string Art_Temporada { get; set; }
        public string temporada { get; set; }
        public Decimal precio { get; set; }
    }
    public class Ent_Articulo_Tipo_Precio
    {
        public string idtipoprecio { get; set; }
        public string descripcion { get; set; }
    }
}
