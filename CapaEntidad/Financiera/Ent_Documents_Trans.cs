using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Documents_Trans
    {
        public bool _check { get; set; }
        public string _docNo { get; set; }
        public string _conceptid { get; set; }
        public string _date { get; set; }
        public decimal _value { get; set; }
        public decimal _increase { get; set; }
        public string _numeroid { get; set; }
        public string _fechadoc { get; set; }
        public Decimal _valuepago { get; set; }

    }
}
