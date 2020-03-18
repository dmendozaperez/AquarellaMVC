using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models.Crystal
{
    public class Liquidation
    {
        #region < Atributos >

        /// <summary>
        /// Informacion de la bodega
        /// </summary>
        public string _wavId { get; set; }
        public string _wavDes { get; set; }
        public string _wavAdd { get; set; }
        public string _wavPhone { get; set; }
        public string _wavUbication { get; set; }

        /// <summary>
        /// Cliente
        /// </summary>
        public string _cusId { get; set; }
        public string _cusDoc { get; set; }
        public string _cusName { get; set; }
        public string _cusAdd { get; set; }
        public string _cusPhone { get; set; }
        public string _cusCellPhone { get; set; }
        public string _cusMail { get; set; }
        public string _cusUbication { get; set; }

        /// <summary>
        /// Liquidacion cabecera
        /// </summary>
        public string _liqNo { get; set; }
        public DateTime _liqDateCreate { get; set; }
        public DateTime _liqDateExp { get; set; }
        public string _liqStatus { get; set; }
        public decimal _liqDctogeneral { get; set; }
        public decimal _liqTaxRate { get; set; }
        public decimal _liqTaxValue { get; set; }
        public decimal _liqHandling { get; set; }
        public decimal _liqpercepcion { get; set; }
        public decimal _liqporcpercepcion { get; set; }
        public decimal _mtoncredito { get; set; }

        public string _idliquidacion { get; set; }
        /// <summary>
        /// Liquidacion detalle
        /// </summary>
        public string _artCode { get; set; }
        public string _artBrand { get; set; }
        public string _artColor { get; set; }
        public string _artName { get; set; }
        public string _artSize { get; set; }
        public decimal _artQty { get; set; }
        public decimal _artPrice { get; set; }
        public decimal _artComm { get; set; }
        public decimal _artDiss { get; set; }

        public decimal _descuento { get; set; }

        //subinforme
        public string _ncredito { get; set; }
        public DateTime _fecha { get; set; }
        public decimal _totalcredito { get; set; }

        //total op

        public decimal _totalop { get; set; }

        public string _Opg { get; set; }

        public Liquidation(string wavId, string wavDes, string wavAdd, string wavPhone, string wavUbication, string cusId,
        string cusDoc, string cusName, string cusAdd, string cusPhone, string cusCellPhone, string cusMail, string cusUbication,
            string liqNo, DateTime liqDateCreate, DateTime liqDateExp, string liqStatus, decimal liqDctogeneral, decimal liqTaxRate,
            decimal liqTaxValue, decimal liqHandling, string artCode, string artBrand, string artColor, string artName,
            string artSize, decimal artQty, decimal artPrice, decimal artComm, decimal artDiss, decimal liqpercepcion,
            decimal liqporcpercepcion, decimal mtoncredito, string ncredito, DateTime fecha, decimal totalcredito, string idliquidacion,
            decimal totalop, decimal descuento, string Opg = "")
        {
            _wavId = wavId;
            _wavDes = wavDes;
            _wavAdd = wavAdd;
            _wavPhone = wavPhone;
            _wavUbication = wavUbication;

            _cusId = cusId;
            _cusDoc = cusDoc;
            _cusName = cusName;
            _cusAdd = cusAdd;
            _cusPhone = cusPhone;
            _cusCellPhone = cusCellPhone;
            _cusMail = cusMail;
            _cusUbication = cusUbication;
            _liqNo = liqNo;
            _liqDateCreate = liqDateCreate;
            _liqDateExp = liqDateExp;
            _liqStatus = liqStatus;
            _liqDctogeneral = liqDctogeneral;
            _liqTaxRate = liqTaxRate;
            _liqTaxValue = liqTaxValue;
            _liqHandling = liqHandling;
            _liqpercepcion = liqpercepcion;
            _liqporcpercepcion = liqporcpercepcion;
            _mtoncredito = mtoncredito;
            _idliquidacion = idliquidacion;

            _ncredito = ncredito;
            _fecha = fecha;
            _totalcredito = totalcredito;

            _artCode = artCode;
            _artBrand = artBrand;
            _artColor = artColor;
            _artName = artName;
            _artSize = artSize;
            _artQty = artQty;
            _artPrice = artPrice;
            _artComm = artComm;
            _artDiss = artDiss;

            _totalop = totalop;
            _descuento = descuento;
            _artDiss = descuento;
            _Opg = Opg;

        }

        #endregion
            }
    public class LiqNcSubinforme
    {
        #region < Atributos >
        public string _liquidacion { get; set; }
        public string _ncredito { get; set; }
        public DateTime _fecha { get; set; }
        public decimal _total { get; set; }


        public LiqNcSubinforme(string liquidacion, string ncredito, DateTime fecha, Decimal total)
        {
            _liquidacion = liquidacion;
            _ncredito = ncredito;
            _fecha = fecha;
            _total = total;
        }

        #endregion
        
    }
    public class VentaPagoSubInforme
    {
        #region < Atributos >
        public string _pago { get; set; }
        public string _documento { get; set; }
        public DateTime _fecha { get; set; }
        public decimal _total { get; set; }


        public VentaPagoSubInforme(string pago, string documento, DateTime fecha, Decimal total)
        {
            _pago = pago;
            _documento = documento;
            _fecha = fecha;
            _total = total;
        }

        #endregion
    }
}