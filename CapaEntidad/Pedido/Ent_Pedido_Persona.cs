using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Ent_Pedido_Persona
    {
        public decimal comision  { get; set; }
        public decimal idCust { get; set; }
        public decimal taxRate { get; set; }
        public decimal commission_POS_visaUnica { get; set; }
        public decimal percepcion { get; set; }
        public string email { get; set; }
        public string nombrecompleto { get; set; }
        public string premio { get; set; }
        public Boolean aplica_percepcion { get; set; }
        public decimal cant_nota { get; set; }

    }    
}
