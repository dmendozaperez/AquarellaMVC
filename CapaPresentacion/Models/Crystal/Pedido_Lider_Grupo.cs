using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models.Crystal
{
    public class Pedido_Lider_Grupo
    {
        #region < ATRIBUTOS >
        /// <summary>
        /// Atributos, Informacion de la venta
        /// </summary>
        public String lider;
        public String lider_documento;
        public String promotor;
        public String promotor_doc;
        public String liq_id;
        public string lider_direccion;
        public decimal cantidad;
        #endregion

        #region < CONSTRUCTOR DE LA CLASE >
        public Pedido_Lider_Grupo(String lider, String lider_documento, string promotor, String promotor_doc,
                 string liq_id, Decimal cantidad, string lider_direccion)
        {
            //
            // TODO: Agregar aquí la lógica del constructor
            //
            _lider = lider;
            _lider_documento = lider_documento;
            _promotor = promotor;
            _promotor_doc = promotor_doc;
            _liq_id = liq_id;
            _cantidad = cantidad;
            _lider_direccion = lider_direccion;
        }
        #endregion

        #region < SETTER's - GETTER's>
        public string _lider_direccion
        {
            get { return lider_direccion; }
            set { lider_direccion = value; }
        }
        public string _lider
        {
            get { return lider; }
            set { lider = value; }
        }
        public String _lider_documento
        {
            get { return lider_documento; }
            set { lider_documento = value; }
        }
        public string _promotor
        {
            get { return promotor; }
            set { promotor = value; }
        }
        public String _promotor_doc
        {
            get { return promotor_doc; }
            set { promotor_doc = value; }
        }
        public String _liq_id
        {
            get { return liq_id; }
            set { liq_id = value; }
        }
        /// <summary>
        ///        
        public Decimal _cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
        #endregion
    }
}