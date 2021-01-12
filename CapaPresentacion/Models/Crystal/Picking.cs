using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models.Crystal
{
    public class Picking
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
        public string _liqStatus { get; set; }

        /// <summary>
        /// Detalle marcacion
        /// </summary>
        public string _artCode { get; set; }
        public string _artBrand { get; set; }
        public string _artColor { get; set; }
        public string _artName { get; set; }
        public string _artSize { get; set; }
        public decimal _artQty { get; set; }

        /// <summary>
        /// Campos especiales
        /// </summary>
        public string _storage { get; set; }
        public string _oP { get; set; }
        public string _empPick { get; set; }
        public string _inst { get; set; }

        public string _lider { get; set; }

        #endregion

        #region < Constructor >

        public Picking(string wavId, string wavDes, string wavAdd, string wavPhone, string wavUbication, string cusId,
        string cusDoc, string cusName, string cusAdd, string cusPhone, string cusCellPhone, string cusMail, string cusUbication,
            string liqNo, string liqStatus, string artCode, string artBrand, string artColor, string artName,
            string artSize, decimal artQty, string storage, string op, string empPick, string inst, string lider)
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
            _liqStatus = liqStatus;

            _artCode = artCode;
            _artBrand = artBrand;
            _artColor = artColor;
            _artName = artName;
            _artSize = artSize;
            _artQty = artQty;

            _storage = storage;
            _oP = op;
            _empPick = empPick;
            _inst = inst;
            _lider = lider;
        }

        #endregion
    }
}