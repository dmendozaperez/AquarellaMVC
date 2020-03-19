using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Pag_Liq
    {
        public string   dtv_transdoc_id { get;set;}
        public string   dtv_concept_id       {get;set;}
        public string   cov_description { get;set;}
        public DateTime document_date_desc {get;set;}
        public DateTime dtd_document_date  {get;set;}
        public decimal  debito              {get;set;}
        public decimal  credito             {get;set;}
        public decimal  val                 {get;set;}
        public string   TIPO                 {get;set;}
        public bool     active                 {get;set;}
        public bool     checks                 {get;set;}
        public decimal  von_increase        {get;set;}
        public decimal  Flag                {get;set;}
    }
}
