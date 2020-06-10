using CapaEntidad.Maestros;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Maestros
{
    public class Dat_Usuario_Tipo
    {
        public List<Ent_Usuario_Tipo> get_lista(Boolean tipo_cliente=false)
        {
            List<Ent_Usuario_Tipo> list = null;
            string sqlquery = "USP_Leer_TipoUsuario";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tipo_cliente", tipo_cliente);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            list = new List<Ent_Usuario_Tipo>();
                            Ent_Usuario_Tipo tipo = new Ent_Usuario_Tipo();
                            tipo.usu_tip_id= "0";
                            tipo.usu_tip_nombre = "--Ninguno--";
                            list.Add(tipo);

                            while (dr.Read())
                            {
                                tipo = new Ent_Usuario_Tipo();
                                tipo.usu_tip_id = dr["Usu_Tip_Id"].ToString();
                                tipo.usu_tip_nombre = dr["Usu_Tip_Nombre"].ToString();
                                list.Add(tipo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;
            }
            return list;
        }

        public List<Ent_Usuario_Tipo> get_lista_tip_user(string tip_user,Boolean tipo_cliente = false)
        {
            List<Ent_Usuario_Tipo> list = null;
            string sqlquery = "USP_MVC_LEER_TIPOUSUARIO";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                       // cmd.Parameters.AddWithValue("@tipo_cliente", tipo_cliente);
                        cmd.Parameters.AddWithValue("@TIP_USER", tip_user);
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            list = new List<Ent_Usuario_Tipo>();
                            Ent_Usuario_Tipo tipo = new Ent_Usuario_Tipo();
                            tipo.usu_tip_id = "0";
                            tipo.usu_tip_nombre = "--Ninguno--";
                            list.Add(tipo);

                            while (dr.Read())
                            {
                                tipo = new Ent_Usuario_Tipo();
                                tipo.usu_tip_id = dr["Usu_Tip_Id"].ToString();
                                tipo.usu_tip_nombre = dr["Usu_Tip_Nombre"].ToString();
                                list.Add(tipo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;
            }
            return list;
        }
    }
}
