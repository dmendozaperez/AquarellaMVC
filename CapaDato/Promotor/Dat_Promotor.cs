using CapaEntidad.Promotor;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace CapaDato.Promotor
{
    public class Dat_Promotor
    {    
        public Boolean InsertarPromotor(Ent_Promotor promotor)
        {
            Boolean valida = false;
            string sqlquery = "USP_Crear_Usuario_MVC";
           
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure; 

                        cmd.Parameters.AddWithValue("@Bas_Primer_Nombre", promotor.prmt_Nombre1);
                        cmd.Parameters.AddWithValue("@Bas_Segundo_Nombre", promotor.prmt_Nombre2);
                        cmd.Parameters.AddWithValue("@Bas_Primer_Apellido", promotor.prmt_ApePater);
                        cmd.Parameters.AddWithValue("@Bas_Segundo_Apellido", promotor.prmt_ApeMater);
                        cmd.Parameters.AddWithValue("@Bas_Fec_nac", promotor.prmt_FecNac);
                        cmd.Parameters.AddWithValue("@Bas_Documento", promotor.prmt_NroDoc);
                        cmd.Parameters.AddWithValue("@Bas_Doc_Tip_Id", promotor.prmt_TipoDoc);
                        cmd.Parameters.AddWithValue("@Bas_Per_Tip_Id", promotor.prmt_TipoPer);
                        cmd.Parameters.AddWithValue("@Bas_Direccion", promotor.prmt_Direc);
                        cmd.Parameters.AddWithValue("@Bas_Telefono", promotor.prmt_Telefono);
                        cmd.Parameters.AddWithValue("@Bas_Fax", promotor.prmt_Fax);
                        cmd.Parameters.AddWithValue("@Bas_Celular", promotor.prmt_Celular);
                        cmd.Parameters.AddWithValue("@Bas_Correo", promotor.prmt_Correo);
                        cmd.Parameters.AddWithValue("@Bas_Are_Id", promotor.prmt_AreaId);
                        cmd.Parameters.AddWithValue("@Bas_Cre_Usuario", promotor.usuId);
                        cmd.Parameters.AddWithValue("@Bas_Sex_Id", promotor.prmt_Sexo);
                        cmd.Parameters.AddWithValue("@Bas_Dis_Id", promotor.prmt_Dist);
                        cmd.Parameters.AddWithValue("@Bas_Usu_TipId", promotor.prmt_UsuTipo);
                        cmd.Parameters.AddWithValue("@Bas_Contraseña", promotor.prmt_contrasenia);
                        cmd.Parameters.AddWithValue("@promotor_defecto", promotor.prmt_PromoDefecto);
                        cmd.Parameters.AddWithValue("@lider", promotor.prmt_Lider);
                        cmd.Parameters.AddWithValue("@bas_agencia", promotor.prmt_Agencia);
                        cmd.Parameters.AddWithValue("@bas_agencia_ruc", promotor.prmt_AgenciaRuc);
                        cmd.Parameters.AddWithValue("@bas_destino", promotor.prmt_Destino);

                        cmd.ExecuteNonQuery();
                        valida = true;
                    }
                }

            }
            catch (Exception exc)
            {
                valida = false;
            }
            return valida;
        }
    }
}
