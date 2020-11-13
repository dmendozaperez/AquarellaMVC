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
    public class Dat_Articulo_Precio
    {
        public List<Ent_Articulo_Precio> buscar_lista(string art_id,string tip)
        {
            List<Ent_Articulo_Precio> listar = null;
            string sqlquery = "[USP_MVC_Buscar_ArticuloPrecio]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@art_id", art_id);
                        cmd.Parameters.AddWithValue("@tip", tip);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            listar = new List<Ent_Articulo_Precio>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Articulo_Precio()
                                      {
                                          tipo = fila["tipo"].ToString(),
                                          tipodes = fila["tipodes"].ToString(),
                                          articulo = fila["articulo"].ToString(),
                                          descripcion = fila["descripcion"].ToString(),
                                          precioigv = Convert.ToDecimal(fila["precioigv"]),
                                          precion = Convert.ToDecimal(fila["precion"]),
                                          Art_Temporada = fila["Art_Temporada"].ToString(),
                                      }
                                    ).ToList();
                        }

                    }
                }
            }
            catch (Exception)
            {
                listar = new List<Ent_Articulo_Precio>();
                
            }
            return listar;
        }

        public List<Ent_Articulo_Tipo_Precio> tipo_precio()
        {
            List<Ent_Articulo_Tipo_Precio> listar = null;
            string sqlquery = "[USP_MVC_LeerTipoPrecio]";
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

                            listar = new List<Ent_Articulo_Tipo_Precio>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Articulo_Tipo_Precio()
                                      {
                                          idtipoprecio = fila["idtipoprecio"].ToString(),
                                          descripcion = fila["descripcion"].ToString(),                                          
                                      }
                                    ).ToList();
                        }

                    }
                }
            }
            catch (Exception)
            {
                listar = new List<Ent_Articulo_Tipo_Precio>();

            }
            return listar;
        }

        public String update_lista_precio(List<Ent_Articulo_Precio> list_update, decimal _usu_id, string _usu_nombre)
        {
            string valida = "";
            string sqlquery = "[USP_MVC_Modificar_ArticuloPrecio]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        DataTable dt_lista_update = dt_update_precio(list_update);
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@usu_id", _usu_id);
                            cmd.Parameters.AddWithValue("@usu_nombre", _usu_nombre);
                            cmd.Parameters.AddWithValue("@tabla_precio", dt_lista_update);
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
            catch(Exception exc)
            {
                valida = exc.Message;
            }
            return valida;
        }

        private DataTable dt_update_precio(List<Ent_Articulo_Precio> listar, Boolean excel = false)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("tipo", typeof(string));
            dt.Columns.Add("Art_Id", typeof(string));
            dt.Columns.Add("Art_Precio", typeof(Decimal));
            dt.Columns.Add("Art_Temporada", typeof(string));
            try
            {
                foreach (var fila in listar)
                {
                    if (!excel)
                    {
                        dt.Rows.Add(fila.tipo, fila.articulo, fila.precion, fila.Art_Temporada);
                    }else
                    {
                        dt.Rows.Add(fila.tipo, fila.articulo, fila.precio, fila.temporada);
                    }
                    
                }

            }
            catch 
            {
                
            }
            return dt;
        }

        public List<Ent_Articulo_Precio> lista_articulo_precio(List<Ent_Articulo_Precio> listar_precio)
        {
            List<Ent_Articulo_Precio> listar = null;
            string sqlquery = "USP_MVC_Buscar_Lista_ArticuloPrecio";
            try
            {
                DataTable dt_lista_update = dt_update_precio(listar_precio,true);
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tmp_precio", dt_lista_update);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Articulo_Precio>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Articulo_Precio()
                                      {
                                          tipo=fila["tipo"].ToString(),
                                          tipodes = fila["tipodes"].ToString(),
                                          articulo = fila["articulo"].ToString(),
                                          descripcion = fila["descripcion"].ToString(),
                                          precioigv =Convert.ToDecimal(fila["precioigv"]),
                                          precion = Convert.ToDecimal(fila["precion"]),
                                          Art_Temporada = fila["Art_Temporada"].ToString(),
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception  exc)
            {
                throw exc;                
            }
            return listar;
        }
    }
}
