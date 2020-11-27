using CapaEntidad.Articulo;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Data.SqlClient;

namespace CapaDato.Articulo
{
    public class Dat_Articulo
    {
        public List<Ent_ListaArticuloPrecio> ListaArticuloPrecio()
        {
            List<Ent_ListaArticuloPrecio> Listar = null;
            string sqlquery = "[USP_MVC_ConsultaListaPrecios]";
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

                            Listar = new List<Ent_ListaArticuloPrecio>();
                            Listar = (from DataRow fila in dt.Rows
                                      select new Ent_ListaArticuloPrecio()
                                      {
                                          IdArticulo = fila["idarticulo"].ToString(),
                                          Cat_Principal = fila["cat_principal"].ToString(),
                                          SubCategoria = fila["Subcategoria"].ToString(),
                                          Marca = fila["Marca"].ToString(),
                                          Descripcion = fila["descripcion"].ToString(),
                                          PrecioIgv = Convert.ToDecimal(fila["precioigv"].ToString()),
                                          PrecioSinIgv = Convert.ToDecimal(fila["preciosinigv"].ToString()),
                                          Costo = Convert.ToDecimal(fila["costo"].ToString())
                                      }
                                    ).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Listar = new List<Ent_ListaArticuloPrecio>();

            }
            return Listar;
        }
    }
}
