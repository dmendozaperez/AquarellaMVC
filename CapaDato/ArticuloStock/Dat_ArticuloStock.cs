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

namespace CapaDato.ReportsValeCompra
{
    public class Dat_ArticuloStock
    {
        public Boolean ba()
        {
            return false;
        }
        public List<Articulo_Stock_Tienda> listar_ArticuloStock(string Cod_Articulo)  
        {
            string sqlquery = "USP_Obtener_Articulo_StockTienda";
            List<Articulo_Stock_Tienda> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {


                        SqlParameter ocodArticulo = cmd.Parameters.Add("@codArticulo", SqlDbType.VarChar);
                        ocodArticulo.Direction = ParameterDirection.Input;
                        ocodArticulo.Value = Cod_Articulo;
                        
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            lista = new List<Articulo_Stock_Tienda>();
                            lista = (from DataRow dr in dt.Rows
                                     select new Articulo_Stock_Tienda()
                                     {

                                         Codigo = dr["Codigo"].ToString(),
                                         Descripcion = dr["Descripcion"].ToString(),
                                         Caracteristica = dr["Caracteristica"].ToString(),
                                         Url_Imagen = dr["Url_Imagen"].ToString(),

                                         Talla = dr["Talla"].ToString(),
                                         Cantidad = Convert.ToInt32(dr["Cantidad"]),

                                         Codigo_tienda = dr["Codigo_tienda"].ToString(),
                                         Nombre_Tienda = dr["Nombre_Tienda"].ToString(),
                                         Direccion_Tienda = dr["Direccion_Tienda"].ToString(),
                                      
                                     }).ToList();

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                lista = null;
            }
            return lista;
        }


        public List<Departamento> listar_Departamento()
        {
            string sqlquery = "USP_Obtener_Articulo_StockDepartamento";
            List<Departamento> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                      
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            lista = new List<Departamento>();
                            lista = (from DataRow dr in dt.Rows
                                     select new Departamento()
                                     {
                                         Dep_Id = dr["DEP_ID"].ToString(),
                                         Dep_Descripcion = dr["DEP_DESCRIPCION"].ToString(),
                                                                                
                                     }).ToList();

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                lista = null;
            }
            return lista;
        }

        public List<Provincia> listar_Provincia(string CodDepartamento)
        {
            string sqlquery = "USP_Obtener_Articulo_StockProvincia";
            List<Provincia> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {

                        SqlParameter ocodDpto = cmd.Parameters.Add("@departamento", SqlDbType.VarChar);
                        ocodDpto.Direction = ParameterDirection.Input;
                        ocodDpto.Value = CodDepartamento;


                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            lista = new List<Provincia>();
                            lista = (from DataRow dr in dt.Rows
                                     select new Provincia()
                                     {
                                         Prv_Cod = dr["PRV_COD"].ToString(),
                                         Prv_Descripcion = dr["PRV_DESCRIPCION"].ToString(),

                                     }).ToList();

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                lista = null;
            }
            return lista;
        }

        public string listarStr_Provincia(string CodDepartamento)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_Obtener_Articulo_StockProvincia", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter odepartamento = oComando.Parameters.Add("@departamento", SqlDbType.VarChar);
                odepartamento.Direction = ParameterDirection.Input;
                odepartamento.Value = CodDepartamento;

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

        public string listarStr_Distrito(string CodDepartamento,string CodProvincia)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_Obtener_Articulo_Stock_Distrito", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter odepartamento = oComando.Parameters.Add("@departamento", SqlDbType.VarChar);
                odepartamento.Direction = ParameterDirection.Input;
                odepartamento.Value = CodDepartamento;

                SqlParameter oprovincia = oComando.Parameters.Add("@provincia", SqlDbType.VarChar);
                oprovincia.Direction = ParameterDirection.Input;
                oprovincia.Value = CodProvincia;

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

        public string listarStr_ArticuloStock(string Cod_Articulo, string Cod_Dpto, string Cod_Prv, string Cod_Dist, string codTalla)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexionPosPeru);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_Obtener_Articulo_StockPorTienda", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter oArticulo = oComando.Parameters.Add("@codArticulo", SqlDbType.VarChar);
                oArticulo.Direction = ParameterDirection.Input;
                oArticulo.Value = Cod_Articulo;

                SqlParameter ocodDepartamento = oComando.Parameters.Add("@codDepartamento", SqlDbType.VarChar);
                ocodDepartamento.Direction = ParameterDirection.Input;
                ocodDepartamento.Value = Cod_Dpto;

                SqlParameter ocodProvincia = oComando.Parameters.Add("@codProvincia", SqlDbType.VarChar);
                ocodProvincia.Direction = ParameterDirection.Input;
                ocodProvincia.Value = Cod_Prv;

                SqlParameter ocodDist = oComando.Parameters.Add("@codDist", SqlDbType.VarChar);
                ocodDist.Direction = ParameterDirection.Input;
                ocodDist.Value = Cod_Dist;

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
