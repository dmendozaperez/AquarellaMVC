using CapaEntidad.Articulo;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Articulo
{
    public class Dat_Promocion
    {
        public List<Ent_Promocion> lista_promocion()
        {
            string sqlquery = "USP_MVC_PROMOCION_LISTA";
            List<Ent_Promocion> listar = null;
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
                            listar = new List<Ent_Promocion>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Promocion()
                                      {
                                          Prom_ID=Convert.ToInt32(fila["Prom_ID"]),
                                          Prom_Des = fila["Prom_Des"].ToString(),
                                          Prom_Porc =Convert.ToDecimal(fila["Prom_Porc"]),
                                          Prom_FecIni =fila["Prom_FecIni"].ToString(),
                                          Prom_FecFin = fila["Prom_FecFin"].ToString(),
                                          Prom_EstID = fila["Prom_EstID"].ToString(),
                                          Prom_Prioridad =Convert.ToInt32(fila["Prom_Prioridad"]),
                                          Prom_Tip_PromID = fila["Prom_Tip_PromID"].ToString(),
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                listar = new List<Ent_Promocion>();
            }
            return listar;
        }

        public List<Ent_Promocion_Config> lista_promocion_config()
        {
            string sqlquery = "USP_MVC_Leer_Promocion_Config";
            List<Ent_Promocion_Config> listar = null;
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
                            listar = new List<Ent_Promocion_Config>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Promocion_Config()
                                      {
                                          Prom_ID = fila["Prom_ID"].ToString(),
                                          Prom_Tipo = fila["Prom_Tipo"].ToString(),
                                          Prom_Des = fila["Prom_Des"].ToString(),
                                          
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {
                listar = new List<Ent_Promocion_Config>();
            }
            return listar;
        }

        public string update_promocion(int estado , int ofe_id ,string ofe_descripcion,Decimal ofe_porc,DateTime ofe_fecini,DateTime ofe_fecfin,
                                       decimal usu,decimal ofe_prioridad,string ofe_prom_id,List<Ent_Articulo>  articulo ,string categoria ,
	                                   string marca ,string estado_promo)
        {
            string sqlquery = "USP_MVC_PROMOCION_UPDATE";
            string valida = "";
            try
            {
                DataTable dt = dt_articulo(articulo);
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State ==0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@estado",estado);
                            cmd.Parameters.AddWithValue("@ofe_id", ofe_id);
                            cmd.Parameters.AddWithValue("@ofe_descripcion", ofe_descripcion);
                            cmd.Parameters.AddWithValue("@ofe_porc", ofe_porc);
                            cmd.Parameters.AddWithValue("@ofe_fecini", ofe_fecini);
                            cmd.Parameters.AddWithValue("@ofe_fecfin", ofe_fecfin);
                            cmd.Parameters.AddWithValue("@usu", usu);
                            cmd.Parameters.AddWithValue("@ofe_prioridad", ofe_prioridad);
                            cmd.Parameters.AddWithValue("@ofe_prom_id", ofe_prom_id);
                            cmd.Parameters.AddWithValue("@articulo", dt);
                            cmd.Parameters.AddWithValue("@categoria", categoria);
                            cmd.Parameters.AddWithValue("@marca", marca);
                            cmd.Parameters.AddWithValue("@estado_promo", estado_promo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception exc)
                    {
                        valida = exc.Message;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }

            }
            catch (Exception exc)
            {
                valida = exc.Message;            
            }
            return valida;
        }
        private DataTable dt_articulo(List<Ent_Articulo> listar)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("articulo", typeof(string));       
            try
            {
                foreach (var fila in listar)
                {                   
                   dt.Rows.Add(fila.Art_Id);                   
                }

            }
            catch
            {

            }
            return dt;
        }

        public List<Ent_Articulo> lista_existe_articulo(List<Ent_Articulo> articulo)
        {
            string sqlquery = "USP_MVC_ARTICULO_EXISTE";            
            List<Ent_Articulo> listar = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        DataTable dt_in= dt_articulo(articulo);
                        
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@articulo", dt_in);

                            using (SqlDataAdapter da=new SqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                listar = new List<Ent_Articulo>();
                                listar = (from DataRow fila in dt.Rows
                                          select new Ent_Articulo()
                                          {
                                              Art_Id=fila["articulo"].ToString(),
                                          }
                                        ).ToList();
                            } 
                           
                        }
                    }
                    catch (Exception ex)
                    {                        
                        throw ex;
                    }
                    
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return listar;
        }

        public Ent_Oferta leer_promocion(string prom_id)
        {
            string sqlquery = "USP_MVC_PROMOCION_CONSULTA";
            Ent_Oferta obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ofe_id", prom_id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            DataTable dt_oferta = ds.Tables[0];
                            DataTable dt_articulo = ds.Tables[1];
                            DataTable dt_categoria = ds.Tables[2];
                            DataTable dt_marca = ds.Tables[3];

                            obj = new Ent_Oferta();
                            obj.Ofe_Id = dt_oferta.Rows[0]["Ofe_Id"].ToString();
                            obj.Ofe_Descripcion = dt_oferta.Rows[0]["Ofe_Descripcion"].ToString();
                            obj.Ofe_Prom_Id = dt_oferta.Rows[0]["Ofe_Prom_Id"].ToString();
                            obj.Ofe_Porc = dt_oferta.Rows[0]["Ofe_Porc"].ToString();
                            obj.Ofe_FecIni = dt_oferta.Rows[0]["Ofe_FecIni"].ToString();
                            obj.Ofe_FecFin = dt_oferta.Rows[0]["Ofe_FecFin"].ToString();
                            obj.ofe_prioridad = dt_oferta.Rows[0]["ofe_prioridad"].ToString();
                            obj.Ofe_EstID = dt_oferta.Rows[0]["Ofe_EstID"].ToString();

                            obj.lista_articulo = (from DataRow fila in dt_articulo.Rows
                                                  select new Ent_Oferta_Articulo()
                                                  {
                                                      Ofe_Id=fila["Ofe_Id"].ToString(),
                                                      Ofe_articulo = fila["articulo"].ToString(),
                                                  }
                                                ).ToList();
                            obj.lista_categoria= (from DataRow fila in dt_categoria.Rows
                                                  select new Ent_Oferta_Categoria()
                                                  {
                                                      Ofe_Id = fila["Ofe_Id"].ToString(),
                                                      Ofe_categoria = fila["categoria"].ToString(),
                                                  }
                                                ).ToList();
                            obj.lista_marca= (from DataRow fila in dt_marca.Rows
                                                   select new Ent_Oferta_Marca()
                                                   {
                                                       Ofe_Id = fila["Ofe_Id"].ToString(),
                                                       Ofe_marca = fila["marca"].ToString(),
                                                   }
                                          ).ToList();

                        }
                    }
                }
            }
            catch(Exception exc)
            {

                throw exc;
            }
            return obj;
        }

    }
}
