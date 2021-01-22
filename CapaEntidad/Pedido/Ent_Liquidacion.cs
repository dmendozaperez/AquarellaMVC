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
        public string liq_tipo_prov { get; set; }
        public string liq_tipo_des { get; set; }
        public string liq_agencia { get; set; }
        public string liq_agencia_direccion { get; set; }
        public string liq_destino { get; set; }
        public string liq_direccion { get; set; }
        public string liq_referencia { get; set; }

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
    public class Ent_Buscar_Pedido
    {
        public string lider { get; set; }
        public string Liq_Id { get; set; }
        public string fecha { get; set; }
        public DateTime? Liq_Fecha { get; set; }
        public int? Bas_Id { get; set; }
        public string nombres { get; set; }
        public string ubicacion { get; set; }
        public string Liq_EstId { get; set; }
        public string Est_Descripcion { get; set; }
        public string estado { get; set; }
        public Decimal? Liq_Igv { get; set; }
        public Decimal? desctogeneral { get; set; }
        public int? cantidad { get; set; }
        public Decimal? descuento { get; set; }
        public Decimal? ganancia { get; set; }
        public Decimal? _base {get;set;}
        public Decimal? valor { get; set; }
        public string Ven_Id { get; set; }
        public string Tra_Descripcion { get; set; }
        public string Tra_Gui_No { get; set; }
        public DateTime? Gru_Fecha { get; set; }
    }

    public class Ent_Picking
    {
        public string Are_Descripcion { get; set; }
        public string Liq_Id { get; set; }
        public DateTime? Liq_Fecha { get; set; }
        public DateTime? Liq_Fecha_Expiracion { get; set; }
        public int? Liq_Basid { get; set; }
        public string Liq_Estid { get; set; }
        public string Est_Descripcion { get; set; }
        public string Nombres { get; set; }
        public string Bas_Direccion { get; set; }
        public string Datedesc { get; set; }
        public DateTime? Cleardate { get; set; }
        public string Cleardesc { get; set; }
        public DateTime? Dateclear { get; set; }
        public Decimal? Liq_Igv { get; set; }
        public int Cantidad { get; set; }
        public int? Pin_Employee { get; set; }
    }

    public class Ent_Picking_info
    {
        public string Liq_Id { get; set; }
        public string Datedesc { get; set; }
        public DateTime? Lhd_Expiration_Date { get; set; }
        public int? Lhn_Customer { get; set; }
        public string Lhv_Status { get; set; }
        public string Stv_Description { get; set; }
        public string Datedesclear { get; set; }
        public DateTime? Dateclear { get; set; }
        public DateTime? Pick_Start { get; set; }
        public string Pick_Startdesc { get; set; }
        public int? Pin_Employee { get; set; }
        public string Nameemployee { get; set; }
        public int? Noliq { get; set; }
        public int? Ldn_Qty { get; set; }
        public string TiempoCorrido { get; set; }
    }

    public class Ent_Pedido_Despacho
    {
        public string Liq { get; set; }
        public string Ven_Id { get; set; }
        public string Fecha { get; set; }
        public string Articulo { get; set; }
        public string Talla { get; set; }
        public int PedOriginal { get; set; }
        public int Pedi_Despachado { get; set; }
        public int Saldo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class Ent_Pedido_Facturacion
    {
        public string Liq_Id { get; set; }
        public string Asesor { get; set; }
        public string Fecha { get; set; }
        public DateTime? Liq_Fecha { get; set; }
        public int? Bas_Id { get; set; }
        public string Liq_Estid { get; set; }
        public string Fechaexpira { get; set; }
        public DateTime? Liq_Fecha_Expiracion { get; set; }
        public Decimal? Liq_Igv { get; set; }
        public string Liq_Gruid { get; set; }
        public string Nombres { get; set; }
        public string Dni_Promotor { get; set; }
        public string Ubicacion { get; set; }
        public int Totalpares { get; set; }
        public int Paq_Cantidad { get; set; }
        public Decimal Liq_Value { get; set; }
        public string Are_Id { get; set; }
        public string Are_Descripcion { get; set; }
        public int? Liq_Guiaid { get; set; }
        public string Tra_Gui_No { get; set; }
        public string Tra_Descripcion { get; set; }
        public string Fecha_Grupo { get; set; }
        public DateTime? Gru_Fecha { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

    public class Ent_Consultar_Pedido
    {
        public string NroDNI { get; set; }
        public string Cliente { get; set; }
        public string NroPedido { get; set; }
        public string FecPedido { get; set; }
        public Decimal? Total { get; set;}
        public string Estado { get; set; }
        public string NroLiquidacion { get; set;}
        public string FecLiquidacion { get; set; }
        public string NroDoc { get; set;}
        public string FecDoc { get; set; }
        public string NroNC { get; set;}
        public string FecNC { get; set; }
        public string Stv_Description { get; set; }
        public string Bas_Documento { get; set; }
    }
}
    
