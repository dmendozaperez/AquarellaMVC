
using CapaEntidad.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Reporte
{
    public class Dat_Combo
    {
       public List<Ent_Combo> get_ListaGrupo()
        {
            List<Ent_Combo> list = null;
            string sqlquery = "Leer_Grupo";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            list = new List<Ent_Combo>();

                            while (dr.Read())
                            {
                                Ent_Combo combo = new Ent_Combo();
                                combo.cbo_codigo = dr["cod_line3"].ToString();
                                combo.cbo_descripcion = dr["des_line3"].ToString();                            
                                
                                list.Add(combo);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }

        public List<Ent_Combo> get_ListaTiendaXstore()
        {
            List<Ent_Combo> list = null;
            string sqlquery = "USP_LISTAR_TIENDA_XSTORE";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            list = new List<Ent_Combo>();

                            while (dr.Read())
                            {
                                Ent_Combo combo = new Ent_Combo();
                                combo.cbo_codigo = dr["CODIGO"].ToString();
                                combo.cbo_descripcion = dr["DESCRIP"].ToString();

                                list.Add(combo);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }

        public string listarStr_ListaCategoria(string strGrupo)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("Leer_Categoria", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter odepartamento = oComando.Parameters.Add("@Grupo", SqlDbType.VarChar);
                odepartamento.Direction = ParameterDirection.Input;
                odepartamento.Value = strGrupo;

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
        public List<Ent_Combo> get_ListaCategoria(string strGrupo)
        {
            List<Ent_Combo> list = null;
            string sqlquery = "Leer_Categoria";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Grupo", strGrupo);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            list = new List<Ent_Combo>();

                            while (dr.Read())
                            {
                                Ent_Combo combo = new Ent_Combo();
                                combo.cbo_codigo = dr["cod_cate3"].ToString();
                                combo.cbo_descripcion = dr["des_cate3"].ToString();

                                list.Add(combo);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }

        public string listarStr_ListaSubCategoria(string strCate)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("Leer_SubCategoria", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter odepartamento = oComando.Parameters.Add("@Categoria", SqlDbType.VarChar);
                odepartamento.Direction = ParameterDirection.Input;
                odepartamento.Value = strCate;

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

        public List<Ent_Combo> get_ListaSubCategoria(string strCate)
        {
            List<Ent_Combo> list = null;
            string sqlquery = "Leer_SubCategoria";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Categoria", strCate);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            list = new List<Ent_Combo>();

                            while (dr.Read())
                            {
                                Ent_Combo combo = new Ent_Combo();
                                combo.cbo_codigo = dr["cod_subc3"].ToString();
                                combo.cbo_descripcion = dr["des_subc3"].ToString();

                                list.Add(combo);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }

        public List<Ent_Combo> get_ListaEstado()
        {
            List<Ent_Combo> list = null;
            string sqlquery = "Leer_Estado";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            list = new List<Ent_Combo>();

                            while (dr.Read())
                            {
                                Ent_Combo combo = new Ent_Combo();
                                combo.cbo_codigo = dr["des_campo1"].ToString();
                                combo.cbo_descripcion = dr["des_campo2"].ToString();

                                list.Add(combo);

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                list = null;
            }
            return list;
        }
    }
}
