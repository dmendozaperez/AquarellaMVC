using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Control
{
    public class Ent_Usuario
    {
        public string usu_nombre { get; set; }
        public string usu_login { get; set; }
        public string usu_contraseña { get; set; }
        public string usu_est_id { get; set; }
        public decimal usu_id { get; set; }
        public string usu_tip_id { get; set; }
        public string usu_area { get; set; }


        public string usu_tip_nom { get; set; }
        public string usu_ip { get; set; }
        public string usu_postPago { get; set; }
        public string usu_nom_ape { get; set;}
    }
}
