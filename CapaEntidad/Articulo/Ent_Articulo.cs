using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Articulo
    {
        public string Art_Id { get; set; }
        public string Art_Descripcion { get; set; }
        public string Art_Mar_Id { get; set; }
        public string Art_Cat_Id { get; set; }
        public string Art_SubCat_Id { get; set; }
        public string Art_Cat_PriId { get; set; }
        public string Art_Foto { get; set; }
        public string Art_Col_Id { get; set; }
        public string Art_Fecha_Crecion { get; set; }
        public Decimal Art_Comision { get; set; }
        public string Art_Gru_Talla { get; set; }
        public string Art_Temporada { get; set; }
        public string Art_Liquidacion { get; set; }
        public Decimal Art_Gru_Ini { get; set; }
        public Decimal Art_Gru_Fin { get; set; }
        public string Art_Fecha_Upd { get; set; }
        public string Art_Catalogo { get; set; }
        public string Art_Foto_Intranet { get; set; }
        //Campos aumentados
        public Decimal precioigv { get; set; }
        public Decimal preciosinigv { get; set; }
        public Decimal costo { get; set; }
        //Campos relacionados
        public Ent_CategoriaPrincipal Ent_CategoriaPrincipal { get; set; }
        /* 
        public Ent_SubCategoria Ent_SubCategoria { get; set; }
        public Ent_Marca Ent_Marca { get; set; }
        */
        public Ent_Articulo_Precio Ent_Articulo_Precio { get; set; }
    }
}
