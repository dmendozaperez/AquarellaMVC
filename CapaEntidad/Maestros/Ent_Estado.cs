using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Maestros
{
    public class Ent_Estado
    {
        public string est_id { get; set; }
        public string est_descripcion { get; set; }
    }

    public class Ent_Estado_Modulo
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        //campos adicionales
        public int Est_Mod_Id { get; set; }
    }
}
