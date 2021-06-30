using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Util
{
    public class Ent_Combo
    {
        public string cbo_codigo { get; set; }
        public string cbo_descripcion { get; set; }

        public string codigo { get; set; }
        public string descripcion { get; set; }
        //LISTA ASESOR LIDER
        public string bas_are_id { get; set; }
        public Decimal bas_id { get; set; }
        public string nombres { get; set; }
        public string bas_aco_id { get; set; }
        public string bas_usu_tipid { get; set; }
        //Departamento y provincia
        public string CodDep { get; set; }
        public string DescripcionDep { get; set; }
        public string CodPrv { get; set; }
        public string DescripcionPrv  { get; set; }
    }
}
