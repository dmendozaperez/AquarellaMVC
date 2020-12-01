using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Articulo
{
    public class Ent_Oferta
    {
        public string Ofe_Id { get; set; }
        public string Ofe_Descripcion { get; set; }
        public string Ofe_Prom_Id { get; set; }
        public string Ofe_Porc { get; set; }
        public string Ofe_FecIni { get; set; }
        public string Ofe_FecFin { get; set; }
        public string ofe_prioridad { get; set; }
        public string Ofe_EstID { get; set; }
        public List<Ent_Oferta_Articulo> lista_articulo { get; set; }
        public List<Ent_Oferta_Categoria> lista_categoria { get; set; }
        public List<Ent_Oferta_Marca> lista_marca { get; set; }

    }
    public class Ent_Oferta_Articulo
    {
        public string Ofe_Id { get; set; }
        public string Ofe_articulo { get; set; }
    }
    public class Ent_Oferta_Categoria
    {
        public string Ofe_Id { get; set; }
        public string Ofe_categoria { get; set; }
    }
    public class Ent_Oferta_Marca
    {
        public string Ofe_Id { get; set; }
        public string Ofe_marca { get; set; }
    }
}
