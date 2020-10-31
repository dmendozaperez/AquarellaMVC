using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Logistica
{
    public class Ent_Lista_Despacho
    {
        public string desp_id { get; set; }
        public string desp_nrodoc { get; set; }
        public string desp_descripcion { get; set; }
        public string totalparesenviado { get; set; }
        public string estado { get; set; }
        public string desp_fechacre { get; set; }
        public string desp_tipo { get; set; } /*L=LIMA; P=PROVINCIA*/
        public string desp_tipo_descripcion { get; set; } /*L=LIMA; P=PROVINCIA*/

    }
    public class Ent_Tipo_Despacho
    {
        public string tip_des_cod { get; set; }
        public string tip_des_nom { get; set; }
    }

    public class Ent_Despacho_Almacen
    {
        public List<Ent_Despacho_Almacen_Cab> despacho_cab { get; set; }
        public List<Ent_Despacho_Almacen_Liquidacion> despacho_liq { get; set; }
    }

    public class Ent_Despacho_Almacen_Cab
    {
        public string Asesor { get; set; }
        public string area_id { get; set; }
        public string NombreLider { get; set; }
        public string Promotor { get; set; }
        public string Rotulo { get; set; }
        public string Distrito { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string Celular { get; set; }
        public string rotulo_courier { get; set; }
        public string Agencia { get; set; }
        public string Destino { get; set; }
        public string Pedido { get; set; }
        public Int32 TotalPares { get; set; }
        public Int32 TotalCatalogo { get; set; }
        public Int32 TotalPremio { get; set; }
        public Int32 TotalCantidad { get; set; }
        public Decimal TotalVenta { get; set; }
        public Decimal Igv { get; set; }
        public string McaFlete { get; set; }
        public string Flete { get; set; }
        public string Lid_Prom { get; set; }   
        public string observacion { get; set; }
        public string detalle { get; set; }

      


    }
    public class Ent_Despacho_Almacen_Liquidacion
    {
        public string area_id { get; set; }
        public string liq_id { get; set; }
        public string lid_prom { get; set; }
        public string bas_tip_des { get; set; }        
    }
    public class Ent_Despacho_Almacen_Update
    {
        public string strIdLider { get; set; }
        public string strIdDetalle { get; set; }        
        public string strLider { get; set; }
        public string strLid_Prom { get; set; }
        public string strPromotor { get; set; }
        public string strPedidos { get; set; }
        public string strRotulo { get; set; }
        public string strRotuloCourier { get; set; }
        public string strPares { get; set; }
        public string strCatalogo { get; set; }
        public string strPremio { get; set; }
        public string strDestino { get; set; }
        public string strAgencia { get; set; }
        public string strMonto { get; set; }
        public string strObs { get; set; }
        public string strDetalle { get; set; }
        public string strMcaCourier { get; set; }
        public string strMcaFlete { get; set; }

        public string strDistrito { get; set; }
        public string strDireccion { get; set; }
        public string strReferencia { get; set; }
        public string strCelular { get; set; }


    }


    public class Ent_Despacho_Almacen_Editar
    {
        public List<Ent_Despacho_Almacen_Cab_Update> Almacen_Cab_Update { get; set; }
        public List<Ent_Despacho_Almacen_Det_Update> Almacen_Det_Update { get; set; }
    }

    public class Ent_Despacho_Almacen_Cab_Update
    {
        public string Desp_NroDoc { get; set; }
        public string Desp_Descripcion { get; set; }
        public string estado { get; set; }
        public string Desp_FechaCre { get; set; }
        public string asesor { get; set; }
        public string NombreLider { get; set; }
        public string Promotor { get; set; }
        public string Rotulo { get; set; }
        public string Rotulo_Courier { get; set; }
        public string Distrito { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string Celular { get; set; }

        public string Agencia { get; set; }
        public string Destino { get; set; }
        public string Pedido { get; set; }
        public Int32 TotalPremio { get; set; }
        public Int32 TotalPremioEnviado { get; set; }
        public Int32 TotalCatalogo { get; set; }
        public Int32 TotalCatalogEnviado { get; set; }
        public Int32 TotalPares { get; set; }
        public Int32 TotalParesEnviado { get; set; }
        public Int32 Total_Cantidad { get; set; }
        public Int32 Total_Cantidad_Envio { get; set; }
        public Decimal TotalVenta { get; set; }
        public string CobroFlete { get; set; }
        public string Courier { get; set; }
        public string Observacion { get; set; }
        public string Detalle { get; set; }
        public string McaCourier { get; set; }
        public string McaFlete { get; set; }
        public Int32 Enviado { get; set; }
        public string Desp_IdDetalle { get; set; }
        public string Desp_id { get; set; }
        public Int32 TotalParesEnviadoEdit { get; set; }
        public Int32 TotalCatalogEnviadoEdit { get; set; }
        public Int32 TotalPremioEnviadoEdit { get; set; }
        public string IdEstado { get; set; }
        public string atendido { get; set; }
        public string IdLider { get; set; }
        public string Lid_Prom { get; set; }

       



    }
    public class Ent_Despacho_Almacen_Det_Update
    {
        public Int32 NroPedidos { get; set; }
        public Int32 NroEnviados { get; set; }
        public Int32 NroPremio { get; set; }
        public Int32 PremioEnviados { get; set; }
        public Int32 CatalogEnviados { get; set; }
        public Int32 CatalogPedidos { get; set; }
        public decimal MontoTotal { get; set; }
        
    }
}
