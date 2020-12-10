using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Concepto
    {
        public string Con_Id { get; set; }
        public string Con_Descripcion { get; set; }
        public string Con_Tip_MovId { get; set; }
        public string Con_Cta_Contable { get; set; }
        public string Con_Cod_Afil { get; set; }
        //campos aumentados
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

}
