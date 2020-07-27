using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Ent_Info_Promotor
    {
        public List<Ent_Liquidacion> liquidacion { get; set; }
        public List<Ent_NotaCredito> notaCredito { get; set; }
        public List<Ent_Consignacioes> consignaciones { get; set; }
        public List<Ent_Saldos> saldos { get; set; }
    }
    public class Ent_Liquidacion
    {
        public string liq_Id { get; set; }
        public string ped_Id { get; set; }
        public string cust_Id { get; set; }
        public string liq_Fecha { get; set; }
        public decimal Pares { get; set; } 
        public string Estado { get; set; }
        public decimal Ganancia { get; set; }
        public decimal Subtotal { get; set; }
        public decimal N_C { get; set; }
        public decimal Total { get; set; }
        public decimal Percepcion { get; set; }
        public decimal TotalPagar { get; set; }
        public string estId { get; set; }
        public string ventaId { get; set; }
        public string liq_opg { get; set; }
    }    
    public class Ent_NotaCredito
    {
        public string Not_Id { get; set; }
        public string Not_Numero { get; set; }
        public string Not_Fecha { get; set; }
        public decimal Not_Det_Cantidad { get; set; }
        public decimal Total { get; set; }
    }
    public class Ent_Consignacioes
    {
        public string Ban_Descripcion { get; set; }
        public string Pag_Num_Consignacion { get; set; }
        public decimal Pag_Monto { get; set; }
        public string Pag_Num_ConsFecha { get; set; }
    }
    public class Ent_Saldos
    {
        public string Descipcion { get; set; }
        public decimal Monto { get; set; }
        public decimal Percepcion { get; set; }
        public decimal Saldo { get; set; }
    }
}
