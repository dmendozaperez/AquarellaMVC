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
    public class Dat_Articulo_Stock
    {
        #region<REGION DE STOCK DE UN ARTICULO>
        public List<Ent_Articulo_Sin_Stock> listar_articulo_sinstock()
        {
            string sqlquery = "USP_MVC_PEDIDO_SIN_STOCK";
            List<Ent_Articulo_Sin_Stock> listar = null;
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
                            listar = new List<Ent_Articulo_Sin_Stock>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Articulo_Sin_Stock()
                                      {
                                          pedido=fila["pedido"].ToString(),
                                          dni = fila["dni"].ToString(),
                                          nombres = fila["nombres"].ToString(),
                                          articulo = fila["articulo"].ToString(),
                                          talla = fila["talla"].ToString(),
                                          estado = fila["estado"].ToString(),
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch 
            {
                listar = new List<Ent_Articulo_Sin_Stock>();
            }
            return listar;
        } 

        public List<Ent_Articulo_Stock> listar_stock(string articulo,ref Ent_Articulo_Info info_articulo)
        {
            string sqlquery = "USP_MVC_CONSULTAR_ARTICULO";
            List<Ent_Articulo_Stock> listar = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@articulo", articulo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            DataTable dtinfo = ds.Tables[0];
                            DataTable dtstk = ds.Tables[1];
                            listar = new List<Ent_Articulo_Stock>();

                            if (ds.Tables.Count==0)
                            {

                            }
                            else
                            { 
                                if (dtinfo.Rows.Count>0)
                                {                                   
                                    info_articulo.codigo = dtinfo.Rows[0]["Codigo"].ToString();
                                    info_articulo.descripcion = dtinfo.Rows[0]["descripcion"].ToString();
                                    info_articulo.marca = dtinfo.Rows[0]["marca"].ToString();
                                    info_articulo.linea = dtinfo.Rows[0]["linea"].ToString();
                                    info_articulo.color = dtinfo.Rows[0]["color"].ToString();
                                    info_articulo.temporada = dtinfo.Rows[0]["temporada"].ToString();
                                    info_articulo.precio =Convert.ToDecimal(dtinfo.Rows[0]["precio"]);
                                    info_articulo.costo =Convert.ToDecimal(dtinfo.Rows[0]["costo"]);
                                    info_articulo.foto = dtinfo.Rows[0]["foto"].ToString();

                                    listar = (from row in dtstk.AsEnumerable()
                                               group row by new
                                               {
                                                   idalmacen = row.Field<string>("id_almacen"),
                                                   almacen = row.Field<string>("almacen"),
                                                   articulo = row.Field<string>("articulo")
                                               } into g
                                               select new Ent_Articulo_Stock()
                                               {
                                                   id_almacen=g.Key.idalmacen,
                                                   almacen = g.Key.almacen,
                                                   articulo = g.Key.articulo,
                                                   total=g.Sum(s=>s.Field<decimal>("cantidad")),
                                                   list_talla = (from DataRow fila in
                                                                     dtstk.AsEnumerable().Where(b => b.Field<string>("almacen") == g.Key.almacen &&
                                                                                                b.Field<string>("articulo") == g.Key.articulo)
                                                                 select new Ent_Articulo_Talla()
                                                                 {
                                                                     talla = fila["talla"].ToString(),
                                                                     cantidad=Convert.ToInt32(fila["cantidad"]),
                                                                 }
                                                                ).ToList(),
                                                   list_pedido_sep= (from DataRow fila in
                                                                      dtstk.AsEnumerable().Where(b => b.Field<string>("almacen") == g.Key.almacen &&
                                                                                                 b.Field<string>("articulo") == g.Key.articulo &&
                                                                                                 b.Field<string>("ped_sep").Length>0
                                                                                                 )
                                                                     select new Ent_Pedido_Sep()
                                                                     {
                                                                         talla = fila["talla"].ToString(),
                                                                         ped_sep = fila["ped_sep"].ToString(),
                                                                     }
                                                                ).ToList()
                                               }                                               
                                               ).ToList();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception exc)
            {
                listar = new List<Ent_Articulo_Stock>();

            }
            return listar;
        }

        #endregion

        #region<REGION DE STOCK DE ARTICULO X CATEGORIA>
        public List<Ent_Articulo_Categoria_Stock> listar_stock_categoria(string categoria,string tempo)
        {
            string sqlquery = "[USP_MVC_LeerStk_CateDet]";
            List<Ent_Articulo_Categoria_Stock> listar = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@cat_pri_id", categoria);
                        cmd.Parameters.AddWithValue("@tempo", tempo);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Articulo_Categoria_Stock>();
                            listar = (from row in dt.AsEnumerable()
                                      group row by new
                                      {
                                          categoria = row.Field<string>("categoria"),
                                          codigo = row.Field<string>("codigo"),
                                          descripcion = row.Field<string>("descripcion"),
                                          foto = row.Field<string>("foto"),
                                          tempo = row.Field<string>("tempo"),
                                          precio = row.Field<Decimal>("precio")
                                      } into g
                                      select new Ent_Articulo_Categoria_Stock()
                                      {
                                          categoria = g.Key.categoria,
                                          codigo = g.Key.codigo,
                                          descripcion = g.Key.descripcion,
                                          foto = g.Key.foto,
                                          tempo = g.Key.tempo,
                                          precio = g.Key.precio,                                          
                                          total = g.Sum(s => s.Field<decimal>("cantidad")),
                                          list_talla = (from DataRow fila in
                                                            dt.AsEnumerable().Where(b => b.Field<string>("categoria") == g.Key.categoria &&
                                                                                         b.Field<string>("codigo") == g.Key.codigo)
                                                        select new Ent_Articulo_Talla()
                                                        {
                                                            talla = fila["talla"].ToString(),
                                                            cantidad = Convert.ToInt32(fila["cantidad"]),
                                                        }
                                                       ).ToList()
                                      }
                                              ).ToList();
                        }

                    }
                }
            }
            catch 
            {
                listar = new List<Ent_Articulo_Categoria_Stock>();                
            }
            return listar;
        }
        #endregion

        #region<MESTROS DE ARTICULO>
        public List<Ent_CategoriaPrincipal>  listar_categoria_principal()
        {
            string sqlquery = "USP_Leer_CategoriaPrincipal";
            List<Ent_CategoriaPrincipal> listar = null;
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
                            listar = new List<Ent_CategoriaPrincipal>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_CategoriaPrincipal()
                                      {
                                          Cat_Pri_Id=fila["Cat_Pri_Id"].ToString(),
                                          Cat_Pri_Descripcion = fila["Cat_Pri_Descripcion"].ToString(),
                                      }).ToList();

                        }
                    }
                }
            }
            catch 
            {
                listar = new List<Ent_CategoriaPrincipal>();

            }
            return listar;
        }

        public List<Ent_Temporada> listar_temporada()
        {
            string sqlquery = "USP_Leer_Temporada";
            List<Ent_Temporada> listar = null;
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
                            listar = new List<Ent_Temporada>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Temporada()
                                      {
                                          cod_tempo = fila["cod_tempo"].ToString(),
                                          des_tempo = fila["des_tempo"].ToString(),
                                      }).ToList();

                        }
                    }
                }
            }
            catch
            {
                listar = new List<Ent_Temporada>();

            }
            return listar;
        }

        #endregion
    }
}
