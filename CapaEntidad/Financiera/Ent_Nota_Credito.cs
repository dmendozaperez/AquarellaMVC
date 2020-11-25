using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Nota_Credito
    {
        public string Not_Id { get; set; }
        public string Not_Fecha { get; set; }
        public string Not_MovId { get; set; }
        public double Not_BasId { get; set; }
        public string Not_EstId { get; set; }
        public double Not_Numero { get; set; }
        public double Not_Usu_Cre { get; set; }
        public double Not_Usu_Mod { get; set; }
        public string Not_Cod_Hash { get; set; }
        public bool Not_Anticipo { get; set; }
        public string Not_Fecha_Ingreso { get; set; }
        public string Not_Estado_Nc { get; set; }
        public string Not_Alm_Id { get; set; }
        public string Not_Url_Pdf { get; set; }
        public string Not_Ven_Id { get; set; }
        public string Not_Numero_Format { get; set; }
        public string Not_VenFec_Ref { get; set; }
        public string Not_Send_Novel { get; set; }
        public string Not_Send_Fecha { get; set; }
        public bool FLAG_WMS { get; set; }
    }
}
