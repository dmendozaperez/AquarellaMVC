using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Estado
    {
        public string Est_Id { get; set; }
        public string Est_Descripcion { get; set; }
        public decimal Est_Mod_Id { get; set; }
        //Campos Adicionales
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
