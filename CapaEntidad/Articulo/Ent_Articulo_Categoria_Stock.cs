using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Articulo_Categoria_Stock
    {
        public string categoria { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }      
        public string foto { get; set; }
        public string tempo { get; set; }
        public Decimal precio { get; set; }
        public Decimal total { get; set; }
        public List<Ent_Articulo_Talla> list_talla { get; set; }

    }
}
