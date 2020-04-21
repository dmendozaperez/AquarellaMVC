using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad.Util;

namespace CapaEntidad.Pedido
{
    public class Ent_Order_Dtl
    {
        public string _idPedido { get; set; }
        public string _code { get; set; }
        public string _artName { get; set; }
        public string _brand { get; set; }
        public string _brandImg { get; set; }
        public string _ArtImg{ get; set; }
        public string _color { get; set; }
        public string _size { get; set; }
        public int _qty { get; set; }
        public int _Stkqty { get; set; }
        public int _qtyCancel { get; set; }
        public string _majorCat { get; set; }
        public string _cat { get; set; }
        public string _subcat { get; set; }
        public string _origin { get; set; }
        public string _originDesc { get; set; }
        public int _comm { get; set; }
        public string _uriPhoto { get; set; }
        public decimal _price { get; set; }
        //varibale del precio de vanta al publico uncluido igv
        public decimal _priceigv { get; set; }
        public string _priceDesc { get; set; }
        //variable de tipo string
        public string _priceigvDesc { get; set; }
        public decimal _commission { get; set; }
        public decimal _commissionigv { get; set; }
        public Decimal _det_dcto_sigv { get; set; }
        public decimal _commissionPctg { get; set; }
        public decimal _Mto_percepcion { get; set; }
        public decimal _Pctg_percepcion { get; set; }
        public string _commissionDesc { get; set; }
        public string _commissionigvDesc { get; set; }
        public decimal _dscto { get; set; }
        public string _dsctoDesc { get; set; }
        public decimal _dsctoPerc { get; set; }
        public decimal _dsctoVale { get; set; }
        public string _dsctoValeDesc { get; set; }
        public string _dsctoMsg { get; set; }
        public decimal _lineTotal { get; set; }
        public decimal _lineTotDesc { get; set; }
       
        public string _ap_percepcion { get; set; }

        public string _ofe_Tipo { get; set; }
        public Decimal _ofe_PrecioPack { get; set; }

        public decimal _ofe_id { get; set; }
        public Decimal _ofe_maxpares { get; set; }
        public Decimal _ofe_porc { get; set; }

        public int _units { get; set; }

        public string _premio { get; set; }

        public string _premioDesc { get; set; }
        public string _premId { get; set; }
        public int id_tran_ofe { get; set; }
        public int nroProms { get; set; }
        public string[] _tallas { get; set; }
        public decimal[] _qtys { get; set; }
    }

    public class Tran_Ofertas
    {
        /* OBJETO EN CLASE Order_dtl.cs */
        public int id { get; set; }
        public string idArt { get; set; }
        public string talla { get; set; }
        public decimal cant { get; set; }
        public decimal ofe_id { get; set; }
        public decimal max_pares { get; set; }
        public decimal ofe_porc { get; set; }
        public string ofe_tipo { get; set; }
        public decimal ofe_artventa { get; set; }
        public decimal ofe_prioridad { get; set; }
        public string hecho { get; set; } = "";
    }


    public class Ent_Detalle
    {
        public string _code             {get;set;}
        public string _size             {get;set;}
        public string _qty              {get;set;}
        public string _price            {get;set;}
        public string _commissionPctg   {get;set;}
        public string _commission       {get;set;}
        public string _ofe_porc         {get;set;}
        public string _dscto            {get;set;}
        public string _ofe_id           { get; set; }
    }

    public class Ent_Nueva_Linea { 
        public string codigoArt     {get;set;}    
        public string DescriArt     {get;set;}
        public string MarcaArt      {get;set;}
        public string ColorArt      {get;set;}
        public decimal PrecioArt     {get;set;}
        public string AfecPercep    {get;set;}
        public string Ofe_tipo      {get;set;}
        public decimal Ofe_Id        {get;set;}
        public string Ofe_MaxPares  {get;set;}
        public decimal Ofe_Porc      {get;set;}
        public string Ofe_ArtVenta  {get;set;}
        public List<Ent_Nueva_Linea_Talla> listTallas { get;set;}
        public decimal Comission     {get;set;}
        public decimal Descuento     {get;set;}
        public decimal Total         {get;set;}
        public decimal CantidadTotal {get;set;}
        public string strUrlImagen  {get;set;}
    }
    public class Ent_Nueva_Linea_Talla
    {
        public string Ofe_tipo          {get;set;}
        public string Ofe_Id            {get;set;}
        public string Ofe_MaxPares      {get;set;}
        public string Ofe_Porc          {get;set;}
        public string Ofe_ArtVenta      {get;set;}
        public string codTalla          {get;set;}
        public int CantTalla         {get;set;}
        public string stockTalla        {get;set;}
        public string comisionTalla     {get;set;}
        public decimal descuentoTalla    {get;set;}
        public string totalTalla { get; set; }
    }

    public class Ent_Order_Hdr
    {
        public int _qtys { get; set; }
        public decimal _subTotal { get; set; }
        public decimal _subTotalOPG { get; set; }
        public string _subTotalOPGDesc { get; set; }
        public decimal _subTotalDesc { get; set; }
        public decimal _taxes { get; set; }
        public decimal _taxesDesc { get; set; } 
        public decimal _grandTotal { get; set; }
        public string _grandTotalDesc { get; set; }
        public decimal _percepcion { get; set; }
        public string _percepciondesc { get; set; }
        public decimal _mtopercepcion { get; set; }
        public string _mtopercepciondesc { get; set; }
        public string _namecompleto { get; set; }
        public string _estadoliqui { get; set; }
        public decimal _mtoncredito { get; set; }
        public string _mtoncreditodesc { get; set; }
        public Int32 _estadoboton { get; set; }
        public string _estadocredito { get; set; }
        public string _premio { get; set; }
    }
    public class Ent_Order_Dtl_Temp
    { 
        public Int32 items { get; set; }
        public string articulo { get; set; }        
        public string talla { get; set; }        
        public Decimal cantidad { get; set; }
        public string premio { get; set; }
        public string premId { get; set; }
    }
}
