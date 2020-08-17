using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Persona
{
    public class Ent_Persona
    {        
        public string Bas_id { get; set; }
        public string Bas_Primer_Nombre { get; set; }
        public string Bas_segundo_nombre { get; set; }
        public string Bas_Primer_Apellido { get; set; }
        public string Bas_Segundo_Apellido { get; set; }
        public string Bas_Documento{ get; set; }
        public string Estado { get; set; }
        public string Bas_Direccion { get; set; }
        public string Bas_Telefono { get; set; }
        public string Bas_Celular { get; set; }
        public string Bas_Correo { get; set; }
        public string bas_are_id { get; set; }
        public string Bas_Sex_Id { get; set; }
        public string Bas_Fec_Nac { get; set; }
        public string Bas_Per_Tip_Id { get; set; }
        public string Usu_Tip_ID { get; set; }
        public string asesor { get; set; }
        public string Are_Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public string NombreCompleto { get; set; }
        public string bas_agencia { get; set; }
        public string bas_destino { get; set; }
        public decimal _commission { get; set; }
        public decimal _taxRate { get; set; }
        public decimal _commission_POS_visaUnica { get; set; }
        public decimal _percepcion { get; set; }
        public Boolean _aplica_percepcion { get; set; }
        public string _vartipopago { get; set; }
        public string premio { get; set; }
        public string desp_cod { get; set; }
        public string desp_des { get; set; }
        public string bas_agencia_direccion { get; set; }
        public string bas_referencia { get; set; }
        public string bas_distrito { get; set; }
        public string bas_tipo_dis { get; set; }

        public string lider_agencia { get; set; }
        public string lider_agencia_direccion { get; set; }
        public string lider_destino { get; set; }
        public string lider_direccion { get; set; }
        public string lider_referencia { get; set; }
        public string lider_distrito { get; set; }

        public string bas_provincia { get; set; }
        public string lider_provincia { get; set; }
        public string lider_bas_tipo_dis { get; set; }

    }
}
