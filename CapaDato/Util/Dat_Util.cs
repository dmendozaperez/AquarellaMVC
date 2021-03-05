using System;
using CapaEntidad.Util;
using CapaEntidad.Control;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Util
{
    public class Dat_Util
    {        
        public List<Departamento> listar_Departamento()
        {
            string sqlquery = "USP_LISTAR_DEPARTAMENTO_MVC";
            List<Departamento> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
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

        public DataSet Listar_Maestros_Promotor()
        {
            DataSet dsReturn = new DataSet();
            string sqlquery = "USP_LEER_DATOS_MAESTROS_MVC";
          
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {

                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                           
                            da.Fill(dsReturn);
                            

                        }
                    }
                }
            }
            catch (Exception exc)
            {

                dsReturn = null;
            }
            return dsReturn;
        }


        public Ent_Promotor_Maestros ListarEnt_Maestros_Promotor(decimal usuarioId)
        {
            DataSet dsReturn = new DataSet();
            string sqlquery = "USP_LEER_DATOS_MAESTROS_MVC";
            List<Ent_Combo> listaDep = null;
            List<Ent_Combo> listaTipoPersona = null;
            List<Ent_Combo> listaTipoUsuario = null;
            List<Ent_Combo> listaTipoDoc = null;
            List<Ent_Combo> listaLider = null;
            Ent_Promotor_Maestros Maestro = new Ent_Promotor_Maestros();
      
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        SqlParameter oCodUsuario = cmd.Parameters.Add("@IdUsuario", SqlDbType.Decimal);
                        oCodUsuario.Direction = ParameterDirection.Input;
                        oCodUsuario.Value = usuarioId;

                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {

                            da.Fill(dsReturn);
                            listaDep = new List<Ent_Combo>();
                            listaDep = (from DataRow dr in dsReturn.Tables[0].Rows
                                     select new Ent_Combo()
                                     {
                                         codigo = dr["Dep_Id"].ToString(),
                                         descripcion = dr["Dep_Descripcion"].ToString(),

                                     }).ToList();

                            listaTipoPersona = new List<Ent_Combo>();
                            listaTipoPersona = (from DataRow dr in dsReturn.Tables[1].Rows
                                        select new Ent_Combo()
                                        {
                                            codigo = dr["per_tip_id"].ToString(),
                                            descripcion = dr["per_tip_descripcion"].ToString(),

                                        }).ToList();

                            listaTipoUsuario = new List<Ent_Combo>();
                            listaTipoUsuario = (from DataRow dr in dsReturn.Tables[2].Rows
                                                select new Ent_Combo()
                                                {
                                                    codigo = dr["Usu_Tip_Id"].ToString(),
                                                    descripcion = dr["Usu_Tip_Nombre"].ToString(),

                                                }).ToList();

                            listaTipoDoc = new List<Ent_Combo>();
                            listaTipoDoc = (from DataRow dr in dsReturn.Tables[3].Rows
                                                select new Ent_Combo()
                                                {
                                                    codigo = dr["Doc_Tip_Id"].ToString(),
                                                    descripcion = dr["Doc_Tip_Descripcion"].ToString(),

                                                }).ToList();

                            listaLider = new List<Ent_Combo>();
                            listaLider = (from DataRow dr in dsReturn.Tables[4].Rows
                                            select new Ent_Combo()
                                            {
                                                codigo = dr["Are_Id"].ToString(),
                                                descripcion = dr["Are_Descripcion"].ToString(),

                                            }).ToList();


                        }
                    }
                }

                Maestro.combo_ListDepartamento = listaDep;
                Maestro.combo_ListTipoPersona = listaTipoPersona;
                Maestro.combo_ListTipoUsuario = listaTipoUsuario;
                Maestro.combo_ListTipoDoc = listaTipoDoc;
                Maestro.combo_ListLider = listaLider;

            }
            catch (Exception exc)
            {

                Maestro = null;
            }
            return Maestro;
        }


        public string listarStr_Provincia(string CodDepartamento)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_FILTRA_PROVINCIA_MVC", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter odepartamento = oComando.Parameters.Add("@dep_id", SqlDbType.VarChar);
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

        public string listarStr_Distrito(string CodProvincia)
        {
            string strJson = "";
            try
            {
                SqlConnection cn = new SqlConnection(Ent_Conexion.conexion);
                cn.Open();
                SqlCommand oComando = new SqlCommand("USP_FILTRA_DISTRITO_MVC", cn);
                oComando.CommandType = CommandType.StoredProcedure;

                SqlParameter odepartamento = oComando.Parameters.Add("@prv_cod", SqlDbType.VarChar);
                odepartamento.Direction = ParameterDirection.Input;
                odepartamento.Value = CodProvincia;

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

        public List<Ent_Combo> Listar_Clientes(Ent_Usuario _Ent)
        {
            List<Ent_Combo> listar = null;
            string sqlquery = "USP_MVC_Leer_Lista_Clientes";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USU", DbType.String).Value = _Ent.usu_id;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Combo>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Combo()
                                      {
                                          codigo = fila["Bas_Id"].ToString(),
                                          descripcion = fila["Nombres"].ToString()
                                      }
                                   ).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

                listar = new List<Ent_Combo>();
            }
            return listar;
        }
    }
}
