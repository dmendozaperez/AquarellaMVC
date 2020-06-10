using CapaEntidad.Cliente;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDato.Cliente
{
    public class Dat_Cliente
    {
        public List<Ent_Cliente_Lista> lista_cliente(string usuario)
        {
            List<Ent_Cliente_Lista> listar = null;
            string sqlquery = "USP_MVC_LEER_CLIENTES";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Cliente_Lista>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Cliente_Lista()
                                      {
                                          dni = fila["dni"].ToString(),
                                          nombres= fila["nombres"].ToString(),
                                          correo = fila["correo"].ToString(),
                                          telefono= fila["telefono"].ToString(),
                                          celular = fila["celular"].ToString(),
                                          direccion= fila["direccion"].ToString(),
                                          Bas_Usu_TipId= fila["Bas_Usu_TipId"].ToString(),
                                          Bas_doc_tip_id = fila["Bas_doc_tip_id"].ToString(),
                                          Bas_Primer_Nombre = fila["Bas_Primer_Nombre"].ToString(),
                                          Bas_Segundo_Nombre = fila["Bas_Segundo_Nombre"].ToString(),
                                          Bas_Primer_Apellido = fila["Bas_Primer_Apellido"].ToString(),
                                          Bas_Segundo_Apellido = fila["Bas_Segundo_Apellido"].ToString(),
                                          Bas_Fec_nac = fila["Bas_Fec_nac"].ToString(),
                                          Bas_Sex_Id = fila["Bas_Sex_Id"].ToString(),

                                          bas_per_tip_id = fila["bas_per_tip_id"].ToString(),
                                          Bas_Correo = fila["Bas_Correo"].ToString(),
                                          bas_telefono = fila["bas_telefono"].ToString(),
                                          bas_celular = fila["bas_celular"].ToString(),
                                          bas_dis_id = fila["bas_dis_id"].ToString(),
                                          Bas_Are_Id = fila["Bas_Are_Id"].ToString(),
                                          Bas_Agencia = fila["Bas_Agencia"].ToString(),
                                          bas_destino = fila["bas_destino"].ToString(),
                                          bas_agencia_ruc = fila["bas_agencia_ruc"].ToString(),

                                          bas_id= fila["bas_id"].ToString(),
                                          bas_aco_id= fila["bas_aco_id"].ToString(),
                                      }
                                   ).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {

                listar = new List<Ent_Cliente_Lista>();
            }
            return listar;
        }

        public List<Ent_Lider_Lista> lista_lider()
        {
            List<Ent_Lider_Lista> listar = null;
            string sqlquery = "USP_MVC_LEER_LISTA_LIDER";
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
                            listar = new List<Ent_Lider_Lista>();
                            listar = (from DataRow fila in dt.Rows
                                      select new Ent_Lider_Lista()
                                      {
                                          bas_are_id= fila["bas_are_id"].ToString(),
                                          bas_id = fila["bas_id"].ToString(),
                                          nombres= fila["nombres"].ToString(),
                                          bas_aco_id = fila["bas_aco_id"].ToString(),                                      
                                      }
                                   ).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

                listar = new List<Ent_Lider_Lista>();
            }
            return listar;
        }

        public string valida_cliente(string dni,string correo)
        {
            string valida = "0";
            string sqlquery = "USP_MVC_VALIDA_CLIENTE";
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
                            cmd.Parameters.AddWithValue("@bas_documento", dni);
                            cmd.Parameters.AddWithValue("@bas_correo", correo);

                            cmd.Parameters.Add("@bas_existe", SqlDbType.VarChar,1);
                            cmd.Parameters["@bas_existe"].Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            valida = cmd.Parameters["@bas_existe"].Value.ToString();
                        }
                    }
                    catch
                    {
                        valida = "3";
                    }
                    if (cn != null)
                        if (cn.State == ConnectionState.Open) cn.Close();
                }
            }
            catch (Exception exc)
            {
                valida ="3";
                
            }
            return valida;
        }

        public string grabar_clientes(Int32 estado,decimal usu_id,Ent_Cliente cliente)
        {
            string valida = "";
            string sqlquery = "USP_MVC_INSERTAR_MODIFICAR_CLIENTE";
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
                            cmd.Parameters.AddWithValue("@estado", estado);
                            cmd.Parameters.AddWithValue("@bas_id", cliente._bas_id);
                            cmd.Parameters.AddWithValue("@Bas_Usu_TipId", cliente._Bas_Usu_TipId);
                            cmd.Parameters.AddWithValue("@Bas_Doc_Tip_Id", cliente._Bas_Doc_Tip_Id);
                            cmd.Parameters.AddWithValue("@Bas_Documento", cliente._Bas_Documento);
                            cmd.Parameters.AddWithValue("@Bas_Primer_Nombre", cliente._Bas_Primer_Nombre);
                            cmd.Parameters.AddWithValue("@Bas_Segundo_Nombre",cliente._Bas_Segundo_Nombre);
                            cmd.Parameters.AddWithValue("@Bas_Primer_Apellido", cliente._Bas_Primer_Apellido);
                            cmd.Parameters.AddWithValue("@Bas_Segundo_Apellido", cliente._Bas_Segundo_Apellido);
                            cmd.Parameters.AddWithValue("@Bas_Fec_nac", cliente._Bas_Fec_nac);
                            cmd.Parameters.AddWithValue("@Bas_Sex_Id", cliente._Bas_Sex_Id);
                            cmd.Parameters.AddWithValue("@Bas_Per_Tip_Id", cliente._Bas_Per_Tip_Id);
                            cmd.Parameters.AddWithValue("@Bas_Correo", cliente._Bas_Correo);
                            cmd.Parameters.AddWithValue("@Bas_Telefono", cliente._Bas_Telefono);
                            cmd.Parameters.AddWithValue("@Bas_Celular", cliente._Bas_Celular);
                            cmd.Parameters.AddWithValue("@Bas_Dis_Id", cliente._Bas_Dis_Id);
                            cmd.Parameters.AddWithValue("@Bas_Direccion", cliente._Bas_Direccion);
                            cmd.Parameters.AddWithValue("@Bas_Are_Id", cliente._Bas_Are_Id);
                            cmd.Parameters.AddWithValue("@bas_agencia", cliente._bas_agencia);
                            cmd.Parameters.AddWithValue("@bas_destino", cliente._bas_destino);
                            cmd.Parameters.AddWithValue("@bas_agencia_ruc", cliente._bas_agencia_ruc);
                            cmd.Parameters.AddWithValue("@Bas_usu", usu_id);

                            if (cliente._Bas_Usu_TipId == "01")
                            {
                                cmd.Parameters.AddWithValue("@Bas_aco_id", cliente._bas_aco_id);
                            }

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

    }
}
