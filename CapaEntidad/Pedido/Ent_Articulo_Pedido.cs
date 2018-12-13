using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Ent_Articulo_pedido
    {

        public string Art_id { get; set; }
        public string Art_Descripcion { get; set; }
        public string Mar_Descripcion { get; set; }
        public string Col_Descripcion { get; set; }
        public string Cat_Pri_Descripcion { get; set; }
        public string Cat_Descripcion { get; set; }
        public string Sca_Descripcion { get; set; }
        public string Art_Comision { get; set; }
        public string Art_Foto { get; set; }
        public string Con_Fig_Percepcion { get; set; }
        public string Afec_Percepcion { get; set; }
        public string Art_Pre_Sin_Igv { get; set; }
        public string Art_Pre_Con_Igv { get; set; }
        public string Art_Costo { get; set; }
        public string Art_Mar_Id { get; set; }
        public string Ofe_Id { get; set; }
        public string Ofe_MaxPares { get; set; }
        public string Ofe_Porc { get; set; }
        public string Tal_Descripcion { get; set; }//campo de talla
        public string Tall_Des { get; set; }//campo de talla
        public string Tall_Cant { get; set; }//campo de talla -- Cantidad por talla

    }    
}
