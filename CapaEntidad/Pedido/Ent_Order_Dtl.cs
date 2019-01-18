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
        public string _code { get; set; }
        /// <summary>
        /// Nombre de articulo
        /// </summary>
        public string _artName { get; set; }
        /// <summary>
        /// Marca
        /// </summary>
        public string _brand { get; set; }
        /// <summary>
        /// Imagen de la marca
        /// </summary>
        public string _brandImg { get; set; }
        /// <summary>
        /// Color
        /// </summary>
        public string _color { get; set; }
        /// <summary>
        /// Talla
        /// </summary>
        public string _size { get; set; }
        /// <summary>
        /// Unidades
        /// </summary>
        public int _qty { get; set; }
        /// <summary>
        /// Unidades cancelAQUARELLAs
        /// </summary>
        public int _qtyCancel { get; set; }
        /// <summary>
        /// Mayor categoria
        /// </summary>
        public string _majorCat { get; set; }
        /// <summary>
        /// Categoria
        /// </summary>
        public string _cat { get; set; }
        /// <summary>
        /// Sub-categoria
        /// </summary>
        public string _subcat { get; set; }
        /// <summary>
        /// Origen
        /// </summary>
        public string _origin { get; set; }
        /// <summary>
        /// Descripcion origen
        /// </summary>
        public string _originDesc { get; set; }
        /// <summary>
        /// Comision de articulo, 1-> si, 0-> No
        /// </summary>
        public int _comm { get; set; }
        /// <summary>
        /// Url de fotografia
        /// </summary>
        public string _uriPhoto { get; set; }
        /// <summary>
        /// Precio publico
        /// </summary>
        public decimal _price { get; set; }
        //varibale del precio de vanta al publico uncluido igv
        public decimal _priceigv { get; set; }

        /// <summary>
        /// Formato moneda del precio publico
        /// </summary>
        public string _priceDesc { get; set; }
        //variable de tipo string
        public string _priceigvDesc { get; set; }
        /// <summary>
        /// Comision valor
        /// </summary>
        public decimal _commission { get; set; }
        public decimal _commissionigv { get; set; }
        public Decimal _det_dcto_sigv { get; set; }
        /// <summary>
        /// % de comision
        /// </summary>
        public decimal _commissionPctg { get; set; }

        /// <summary>
        /// Formato de moneda del valor de la comision
        /// </summary>
        public string _commissionDesc { get; set; }
        public string _commissionigvDesc { get; set; }
        /// <summary>
        /// Valor descuento sobre el item
        /// </summary>
        public decimal _dscto { get; set; }
        /// <summary>
        /// Formato moneda del valor del descuento item
        /// </summary>
        public string _dsctoDesc { get; set; }
        /// <summary>
        /// % de descuento item
        /// </summary>
        public decimal _dsctoPerc { get; set; }
        /// <summary>
        /// Valor de descuento item
        /// </summary>
        public decimal _dsctoVale { get; set; }
        /// <summary>
        /// Formato moneda del valor del descuento
        /// </summary>
        public string _dsctoValeDesc { get; set; }
        /// <summary>
        /// Mensaje del descuento
        /// </summary>
        public string _dsctoMsg { get; set; }
        /// <summary>
        /// Total neto de la linea
        /// </summary>
        public decimal _lineTotal { get; set; }
        /// <summary>
        /// Formato moneda del total de la linea
        /// </summary>
        public string _lineTotDesc { get; set; }

        public string _ap_percepcion { get; set; }

        public string _ofe_Tipo { get; set; }
        public Decimal _ofe_PrecioPack { get; set; }

        public decimal _ofe_id { get; set; }
        public Decimal _ofe_maxpares { get; set; }
        public Decimal _ofe_porc { get; set; }

        /// <summary>
        /// Unidades
        /// </summary>
        public int _units { get; set; }

        public string _premio { get; set; }

        public string _premioDesc { get; set; }
        public string _premId { get; set; }

    }    
}
