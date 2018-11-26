using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.ArticuloStock
{
    public class Articulo_Stock_Tienda
    {
        //datos del articulo
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Caracteristica { get; set; }
        public string Url_Imagen { get; set; }
        public string Url_Imagen_susti { get; set; }
        //datos del Stock
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        //datos de la tienda
        public string Codigo_tienda { get; set; }
        public string Nombre_Tienda { get; set; }
        public string Direccion_Tienda { get; set; }
       
       
       

    }
}
