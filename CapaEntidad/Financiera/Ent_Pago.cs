using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Pago
    {
        public string Pag_Id { get; set; }
        public double Pag_BasId { get; set; }
        public string Pag_BanId { get; set; }
        public string Pag_Num_Consignacion { get; set; }
        public string Pag_Num_ConsFecha { get; set; }
        public string Pag_Fecha_Creacion { get; set; }
        public double Pag_Monto { get; set; }
        public string Pag_Comentario { get; set; }
        public string Pag_EstId { get; set; }
        public string Pag_Fecha_Evalua { get; set; }
        public string Pag_ConId { get; set; }
        public string Pag_Num_Tarjeta { get; set; }
        public double Pag_Usu_Creacion { get; set; }
        public string Pag_Pedido { get; set; }
    }
}
