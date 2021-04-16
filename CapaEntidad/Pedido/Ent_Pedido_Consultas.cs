using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Pedido
{
    public class Ent_Pedido_Pagados
    {
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string Promotor { get; set; }
        public string Dni { get; set; }
        public string Ubicacion { get; set; }
        public string Pedido { get; set; }
        public string Tipo_Estado { get; set; }
        public string Fecha_Cruce { get; set; }
        public string Estado_Pedido { get; set; }
        public string Delivery { get; set; }
        public string Agencia { get; set; }
        public string Destino { get; set; }
        //campos adicionales
        public decimal Usu_Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
