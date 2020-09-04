using CapaEntidad.Control;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Control
{
    public class Dat_Recov_Pass
    {
        public Ent_Recov_Pass get_recov_pass(string _usv_username, string documento)
        {
            string sqlquery = "USP_Envia_CorreoRecuperaPassword";
            Ent_Recov_Pass recov = null;
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
                            cmd.Parameters.AddWithValue("@nombreUsuario", _usv_username);
                            cmd.Parameters.AddWithValue("@numeroDocumento", documento);

                            SqlDataReader dr = cmd.ExecuteReader();

                            if (dr.HasRows)
                            {
                                recov = new Ent_Recov_Pass();
                                while(dr.Read())
                                {
                                    recov.COD_ERROR = dr["COD_ERROR"].ToString();
                                    recov.DESCRIPCION_ERROR = dr["DESCRIPCION_ERROR"].ToString();
                                }
                            }

                        }
                    }
                    catch (Exception)
                    {
                        recov = null;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();

                }
            }
            catch 
            {
                recov = null;
            }
            return recov;
        }


        public Ent_Recov_Pass recovery_pass(string password, string codigo)
        {
            string sqlquery = "USP_MVC_RecuperaPassword";
            Ent_Recov_Pass recov = null;
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
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@codigo", codigo);

                            SqlDataReader dr = cmd.ExecuteReader();

                            if (dr.HasRows)
                            {
                                recov = new Ent_Recov_Pass();
                                while (dr.Read())
                                {
                                    recov.COD_ERROR = dr["COD_ERROR"].ToString();
                                    recov.DESCRIPCION_ERROR = dr["DESCRIPCION_ERROR"].ToString();
                                }
                            }

                        }
                    }
                    catch (Exception)
                    {
                        recov = null;
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();

                }
            }
            catch
            {
                recov = null;
            }
            return recov;
        }
    }
}
