using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Admonred
{
   public class Ent_Cordinator
    {
        public string _vartipopago { get; set; }
        public string _co { get; set; }
        public decimal _idCust { get; set; }
        public string _usuTipo { get; set; }
        public decimal _commission { get; set; }
        public string _idWare { get; set; }
        public decimal _taxRate { get; set; }
        public decimal _commission_POS_visaUnica { get; set; }
        public decimal _percepcion { get; set; }
        public decimal _mtopercepcion { get; set; }
        public string _email { get; set; }
        public string _nombrecompleto { get; set; }
        public string _premio { get; set; }
        public string _ppremio { get; set; }
        public string _pTalla { get; set; }
        public Int32 _pCantidad { get; set; }
        public Decimal _pMonto { get; set; }
        public string _pPremID { get; set; }
        public Boolean _aplica_percepcion { get; set; }
        public decimal _mtoimporte { get; set; }
    }
}
