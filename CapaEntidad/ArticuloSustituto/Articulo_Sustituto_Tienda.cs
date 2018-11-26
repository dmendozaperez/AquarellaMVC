using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.ArticuloSustituto

{
    public class Articulo_Sustituto_Tienda
    {
        //datos del articulo
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Caracteristica { get; set; }
        public string Url_Imagen { get; set; }
        public string Url_Imagen_susti { get; set; }
        //datos de articulo sustituto
        public string Talla { get; set; }
        public int Cantidad { get; set; }
        public string Codigo_Susti { get; set; }
        public string Nombre_Susti { get; set; }
        public string Direccion_Susti { get; set; }
       
       
       

    }
}
