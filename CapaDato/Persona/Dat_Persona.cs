using System;
using CapaEntidad.Util;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Persona
{
    public class Dat_Persona
    {       
       
        public string strBuscarPersona(string nroDocumento)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_LEER_PERSONA_USUARIO_MVC", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter oDocumento = oComando.Parameters.Add("@bas_documento", SqlDbType.VarChar);
                oDocumento.Direction = ParameterDirection.Input;
                oDocumento.Value = nroDocumento;

                SqlDataReader oReader = oComando.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dataTable = new DataTable("row");
                dataTable.Load(oReader);

                strJson = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
                strJson = strJson.Replace(Environment.NewLine, "");
                //strJson = strJson.Replace(" ", "");
                cn.Close();
            }
            catch (Exception ex)
            {
                return strJson;
            }
            //return oLista;
            return strJson;
        }


    }
}
