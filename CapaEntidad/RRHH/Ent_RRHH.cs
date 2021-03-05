using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.RRHH
{
    public class Ent_Promotor_Lider
    {
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string Promotor { get; set; }
        public string Documento { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
        public string Fecing { get; set; }
        public DateTime? Fecactv { get; set; }
        public string Fec_Nac { get; set; }
        public string Zona { get; set; }
        public string Activo { get; set; }
        //Campos Adicionales
        public string Bas_Id { get; set; }
        public string Are_Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
