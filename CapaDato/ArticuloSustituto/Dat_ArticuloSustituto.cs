using System;
using CapaEntidad.ArticuloStock;
using CapaEntidad.Util;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Articulosustituto
{
    public class Dat_ArticuloSustituto
    {
        
        public string listarStr_ArticuloSustituto(string Cod_tda, string Cod_Articulo, string codTalla)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_OBTENER_ARTICULO_SUSTITUTO", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter oArticulo = oComando.Parameters.Add("@codArticulo", SqlDbType.VarChar);
                oArticulo.Direction = ParameterDirection.Input;
                oArticulo.Value = Cod_Articulo;

                SqlParameter oCodTda = oComando.Parameters.Add("@codTda", SqlDbType.VarChar);
                oCodTda.Direction = ParameterDirection.Input;
                oCodTda.Value = Cod_tda;

                SqlParameter ocodTalla = oComando.Parameters.Add("@codTalla", SqlDbType.VarChar);
                ocodTalla.Direction = ParameterDirection.Input;
                ocodTalla.Value = codTalla;

                SqlDataReader oReader = oComando.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dataTable = new DataTable("row");
                dataTable.Load(oReader);

                strJson = JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
                strJson = strJson.Replace(Environment.NewLine, "");
                //strJson = strJson.Replace(" ", "");
                cn.Close();
            }
            catch (Exception ex) {

                return strJson;
            }

            //return oLista;
            return strJson;
        }

    }
}
