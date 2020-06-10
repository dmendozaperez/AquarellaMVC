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
    public class Dat_Combo_Lider
    {
        public List<Ent_Combo_Lider> lista_lider(string usu_tip_id="-1",string usu_id="")
        {
            List<Ent_Combo_Lider> listar = null;
            string sqlquery = "USP_MVC_LEER_LIDER";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@usu_tip_id", usu_tip_id);
                        cmd.Parameters.AddWithValue("@usu_id", usu_id);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            listar = new List<Ent_Combo_Lider>();

                            List<Ent_Combo_Lider> lid_d = new List<Ent_Combo_Lider>();
                            Ent_Combo_Lider lid = new Ent_Combo_Lider();
                            lid.cod_lider = "0";
                            lid.nom_lider = "--Ninguno--";
                            lid_d.Add(lid);

                            listar = (
                                    from DataRow fila in dt.Rows
                                      select new Ent_Combo_Lider()
                                      {
                                          cod_lider= fila["cod_lider"].ToString(),
                                          nom_lider= fila["nom_lider"].ToString(),
                                          
                                      }
                                   ).ToList();

                            listar = lid_d.Union(listar).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

                listar = new List<Ent_Combo_Lider>();
            }
            return listar;
        }

        public List<Ent_Combo_Asesor> lista_asesor()
        {
            List<Ent_Combo_Asesor> listar = null;
            string sqlquery = "USP_MVC_LEER_ASESOR";
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
                            listar = new List<Ent_Combo_Asesor>();

                            List<Ent_Combo_Asesor> lid_d = new List<Ent_Combo_Asesor>();
                            Ent_Combo_Asesor lid = new Ent_Combo_Asesor();
                            lid.Ase_Id = "0";
                            lid.Ase_Nom = "--Ninguno--";
                            lid_d.Add(lid);

                            listar = (
                                    from DataRow fila in dt.Rows
                                    select new Ent_Combo_Asesor()
                                    {
                                        Ase_Id = fila["Ase_Id"].ToString(),
                                        Ase_Nom = fila["Ase_Nom"].ToString(),
                                    }
                                   ).ToList();

                            listar = lid_d.Union(listar).ToList();
                        }
                    }
                }
            }
            catch (Exception exc)
            {

                listar = new List<Ent_Combo_Asesor>();
            }
            return listar;
        }

    }
}
