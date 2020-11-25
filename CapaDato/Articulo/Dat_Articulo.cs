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
        public List<Ent_Articulo> ListaPrecios()
        {
            List<Ent_Articulo> Listar = new List<Ent_Articulo>();
            string sqlquery = "[USP_MVC_ConsultaListaPrecios]";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader _Leer = cmd.ExecuteReader())
                        {
                            if (_Leer.HasRows)
                            {
                                while (_Leer.Read())
                                {
                                    Ent_Articulo Item = new Ent_Articulo
                                    {
                                        Art_Id = (string)(_Leer["idarticulo"]),
                                        Ent_CategoriaPrincipal = new Ent_CategoriaPrincipal
                                        {
                                            Cat_Pri_Descripcion = (string)(_Leer["cat_principal"])
                                        },
                                        Ent_SubCategoria = new Ent_SubCategoria
                                        {
                                            Sca_Descripcion = (string)(_Leer["Subcategoria"])
                                        },
                                        Ent_Marca = new Ent_Marca
                                        {
                                            Mar_Descripcion = (string)(_Leer["Marca"])
                                        },
                                        Art_Descripcion = (string)(_Leer["descripcion"]),
                                        precioigv = (Decimal)(_Leer["precioigv"]),
                                        preciosinigv = (Decimal)(_Leer["preciosinigv"]),
                                        costo = (Decimal)(_Leer["costo"])
                                    };
                                    Listar.Add(Item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            return Listar;
        }
    }
}
