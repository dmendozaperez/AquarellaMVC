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
    public class Dat_Premios
    {
        public List<Ent_Premios> lista_premios()
        {
            List<Ent_Premios> listar = null;
            string sqlquery = "[USP_MVC_Leer_Premios]";
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

                            listar = new List<Ent_Premios>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Premios()
                                      {
                                          id = Convert.ToInt32(fila["id"]),
                                          monto =Convert.ToDecimal(fila["monto"]),
                                          descripcion = fila["descripcion"].ToString(),
                                          stock = Convert.ToInt32(fila["stock"]),
                                          stockingresado = Convert.ToInt32(fila["stockingresado"]),
                                        
                                      }
                                    ).ToList();
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                listar = new List<Ent_Premios>();

            }
            return listar;
        }
        public List<Ent_Premios_Articulo> lista_premios_articulo(Int32 id)
        {
            List<Ent_Premios_Articulo> listar = null;
            string sqlquery = "[USP_MVC_Consulta_Premio_Articulo]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdPremio",id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            listar = new List<Ent_Premios_Articulo>();

                            listar = (from row in dt.AsEnumerable()
                                      group row by new
                                      {                                         
                                          articulo = row.Field<string>("articulo")
                                      } into g
                                      select new Ent_Premios_Articulo()
                                      {
                                         
                                          articulo = g.Key.articulo,
                                          total = g.Sum(s => s.Field<Int32>("cantidad")),
                                          list_talla = (from DataRow fila in
                                                            dt.AsEnumerable().Where(b => b.Field<string>("articulo") == g.Key.articulo)
                                                        select new Ent_Articulo_Talla()
                                                        {
                                                            talla = fila["talla"].ToString(),
                                                            cantidad = Convert.ToInt32(fila["cantidad"]),
                                                        }
                                                       ).ToList(),
                                      }).ToList();
                           
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                listar = new List<Ent_Premios_Articulo>();

            }
            return listar;
        }
        public List<Ent_Lista_PremiosXArticulos> lista_premiosXarticulo(Int32 id)
        {
            List<Ent_Lista_PremiosXArticulos> listar = null;
            string sqlquery = "[USP_MVC_Leer_Premio_Articulo]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdPremio", id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            listar = new List<Ent_Lista_PremiosXArticulos>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Lista_PremiosXArticulos()
                                      {
                                          id=Convert.ToInt32(fila["id"]),
                                          articulo = fila["articulo"].ToString(),
                                          talla = fila["talla"].ToString(),
                                          cantidad = Convert.ToInt32(fila["cantidad"]),                                       
                                          entregado = Convert.ToInt32(fila["entregado"]),
                                          stock = Convert.ToInt32(fila["stock"]),

                                      }
                                    ).ToList();
                        }

                    }
                }
            }
            catch (Exception)
            {
                listar = new List<Ent_Lista_PremiosXArticulos>();

            }
            return listar;
        }
        public string eliminar_articulo_premio(Int32 id,decimal usu)
        {
            string sqlquery = "[USP_MVC_Eliminar_ArticuloPremio]";
            string valida = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PremArt_Id", id);
                            cmd.Parameters.AddWithValue("@Usu", usu);
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
        public List<Ent_Premios_Articulo_Stock> lista_premios_articulo_stock(string art_id)
        {
            List<Ent_Premios_Articulo_Stock> listar = null;
            string sqlquery = "[USP_MVC_Leer_Premio_Articulo_Stock]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@art_id", art_id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            listar = new List<Ent_Premios_Articulo_Stock>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Premios_Articulo_Stock()
                                      {
                                          articulo = fila["articulo"].ToString(),
                                          talla = fila["talla"].ToString(),
                                          stock = Convert.ToInt32(fila["cantidad"]),
                                          
                                      }
                                    ).ToList();
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                listar = new List<Ent_Premios_Articulo_Stock>();

            }
            return listar;
        }

        public string insertar_articulo_premio(Int32 id,string detalle, decimal usu)
        {
            string sqlquery = "[USP_MVC_Insertar_ArticuloPremio]";
            string valida = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    try
                    {
                        if (cn.State == 0) cn.Open();
                        using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdPremio", id);
                            cmd.Parameters.AddWithValue("@strListDetalle", detalle);
                            cmd.Parameters.AddWithValue("@UsuCrea", usu);
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

      

    }
}
