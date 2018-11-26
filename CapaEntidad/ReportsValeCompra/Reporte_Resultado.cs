using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.ReportsValeCompra
{
    public class Reporte_Resultado
    {
        
        public string Institucion { get; set; }
        public string rep_CupBarra { get; set; }
        public string Codigo { get; set; }
        public string rep_CupNumero { get; set; }
        public string Numero { get; set; }
        public string rep_CupMonto { get; set; }
        public string soles { get; set; }
        public string rep_CupEstado { get; set; }
        public string Estado { get; set; }
        
       
        public string rep_tdaCodigo { get; set; }
        public string Codigo_tda { get; set; }
        public string rep_tdaDesc { get; set; }
        public string Desc_tda { get; set; }
        public string rep_tdaDirec { get; set; }
        
        
        public string rep_docTipo { get; set; }
        public string rep_docSerie { get; set; }
        public string Serie { get; set; }
        public string rep_docNro { get; set; }
        public string Correlativo { get; set; }
        public string Documento { get; set; }
        public string rep_docfecha { get; set; }
        public string Fecha_doc { get; set; }

        public string rep_dni { get; set; }
        public string DNI { get; set; }
        public string Cliente { get; set; }
        public string rep_nombre { get; set; }
        public string rep_apellidoPater { get; set; }
        public string rep_apellidoMater { get; set; }
        public string rep_email { get; set; }
        public string total_disponible { get; set; }
        public string total_consumido { get; set; }





    }
}
