using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Order_Dtl_Temp
    {
        /// numero de item de la fila
        /// </summary>
        public Int32 items { get; set; }
        /// <summary>
        /// codigo de articulo
        /// </summary>
        public string articulo { get; set; }
        /// <summary>
        /// talla del articulo
        /// </summary>
        public string talla { get; set; }
        /// <summary>
        /// cantidad del producto
        /// </summary>
        public Decimal cantidad { get; set; }

        public string premio { get; set; }

        public string premId { get; set; }

    }    
}
