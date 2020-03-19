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
        public decimal Art_Comision { get; set; }
        public string Art_Foto { get; set; }
        public decimal Con_Fig_Percepcion { get; set; }
        public decimal Afec_Percepcion { get; set; }
        public decimal Art_Pre_Sin_Igv { get; set; }
        public decimal Art_Pre_Con_Igv { get; set; }
        public decimal Art_Costo { get; set; }
        public string Art_Mar_Id { get; set; }
        public decimal Ofe_Id { get; set; }
        public decimal Ofe_MaxPares { get; set; }
        public decimal Ofe_Porc { get; set; }
        public string Ofe_Tipo { get; set; }
        public decimal Ofe_ArtVenta { get; set; }
        public decimal Ofe_Prioridad { get; set; }
        public string Tal_Descripcion { get; set; }//campo de talla
        public string Tall_Des { get; set; }//campo de talla
        public string Tall_Cant { get; set; }//campo de talla -- Cantidad por talla
        public string _premId { get; set; }
        public string _ap_percepcion { get; set; }
        public string _premioDesc { get; set; }
        public List<Ent_Articulo_Ofertas> _ofertas {get;set;}
    }

    public class Ent_Articulo_Tallas
    {
        public string Stk_ArtId { get; set; }
        public decimal Tal_Descripcion { get; set; }
        public decimal Tall_Des { get; set; }
        public decimal Tall_Cant { get; set; }
    }
    public class Ent_Articulo_Ofertas
    {
        public decimal Ofe_Id { get; set; }
        public decimal Ofe_MaxPares { get; set; }
        public decimal Ofe_Porc { get; set; }
        public string Ofe_Tipo { get; set; }
        public decimal Ofe_ArtVenta { get; set; }
        public decimal Ofe_Prioridad { get; set; }
    }
}
