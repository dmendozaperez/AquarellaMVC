using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Cliente
{
    public class Ent_Cliente_Lista
    {
        public string dni { get; set; }
        public string nombres { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public string direccion { get; set; }
    }
    public class Ent_Lider_Lista
    {
        public string bas_are_id { get; set; }
        public string bas_id { get; set; }
        public string nombres { get; set; }
        public string bas_aco_id { get; set; }
    }
    public class Ent_Cliente
    {
        public string _bas_id { get; set; }
        public string _Bas_Usu_TipId { get; set; }
        public string _Bas_Doc_Tip_Id { get; set; }
        public string _Bas_Documento { get; set; }
        public string _Bas_Primer_Nombre { get; set; }
        public string _Bas_Segundo_Nombre { get; set; }
        public string _Bas_Primer_Apellido { get; set; }
        public string _Bas_Segundo_Apellido { get; set; }
        public string _Bas_Fec_nac { get; set; }
        public string _Bas_Sex_Id { get; set; }
        public string _Bas_Per_Tip_Id { get; set; }
        public string _Bas_Correo { get; set; }
        public string _Bas_Telefono { get; set; }
        public string _Bas_Celular { get; set; }
        public string _Bas_Dis_Id { get; set; }
        public string _Bas_Direccion { get; set; }
        public string _Bas_Are_Id { get; set; }
        public string _bas_agencia { get; set; }
        public string _bas_destino { get; set; }
        public string _bas_agencia_ruc { get; set; }
       
    }
}
