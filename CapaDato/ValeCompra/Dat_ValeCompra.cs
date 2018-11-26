using CapaEntidad.ValeCompra;
using CapaEntidad.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CapaDato.ValeCompra
{
    public class Dat_ValeCompra
    {

        public Ent_ValeCompra valeCompra { get; set; }


        public List<Ent_ValeCompra> get_lista()
        {
            List<Ent_ValeCompra> list = null;
            string sqlquery = "USP_Leer_ValesCompra";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            list = new List<Ent_ValeCompra>();
                            list = (from DataRow dr in dt.Rows
                                     select new Ent_ValeCompra()
                                     {

                                         valCompra_id = Int32.Parse(dr["VCom_ID"].ToString()),
                                            valCompra_generado = dr["VCom_Generado"].ToString(),
                                            valCompra_ruc = dr["VCom_Ruc"].ToString(),
                                            valCompra_razon = dr["VCom_Razon"].ToString(),
                                            valCompra_descripcion = dr["VCom_Descripcion"].ToString(),
                                            valCompra_fecVigenIni = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dr["VCom_FecIniVigencia"])),
                                            valCompra_fecVigenFin = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dr["VCom_FecFinVigencia"])),
                                            valCompra_usuCreacion = dr["VCom_UsuarioCreacion"].ToString(),
                                            valCompra_fecCreacion = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dr["VCom_FecCreacion"])),
                                            valCompra_ipCreacion = dr["VCom_IpCreacion"].ToString(),

                                     }).ToList();

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


        public Ent_ValeCompra ConsultarVales(string strId)
        {
            Ent_ValeCompra _valeCompra = null;
            List<Ent_ValeCompraDetalle> lisDet = null;
            string strRuc = "";
            string strRazon = "";
            string strComcepto = "";
            string strFecIni = "";
            string strFecFin = "";
            string strGenerado = "";
            string strTotal = "";

            string sqlquery = "USP_Leer_ValesCompra_detalle";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VCom_ID", strId);
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            _valeCompra = new Ent_ValeCompra();
                            lisDet = new List<Ent_ValeCompraDetalle>();



                            while (dr.Read())
                            {
                                Ent_ValeCompraDetalle _valeCompraDetalle = new Ent_ValeCompraDetalle();

                                strRuc = dr["VCom_Ruc"].ToString();
                                strTotal = dr["Vcom_Total"].ToString();
                                strRazon = dr["VCom_Razon"].ToString();
                                strComcepto = dr["VCom_Descripcion"].ToString();
                                strFecIni = dr["VCom_FecIniVigencia"].ToString();
                                strFecFin = dr["VCom_FecFinVigencia"].ToString();
                                strGenerado = dr["VCom_Generado"].ToString();
                                _valeCompraDetalle.valCompra_id = Int32.Parse(dr["VCom_ID"].ToString());
                                _valeCompraDetalle.valComDet_monto = Convert.ToDecimal(dr["VComDet_Monto"].ToString());
                                _valeCompraDetalle.valComDet_cantidad = Int32.Parse(dr["VComDet_Cantidad"].ToString());

                                lisDet.Add(_valeCompraDetalle);

                            }

                            _valeCompra.valCompra_id = Int32.Parse(strId); ;
                            _valeCompra.valCompra_ruc = strRuc;
                            _valeCompra.valCompra_generado = strGenerado;
                            _valeCompra.valCompra_razon = strRazon;
                            _valeCompra.valCompra_descripcion = strComcepto;
                            _valeCompra.valCompra_fecVigenIni = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(strFecIni));
                            _valeCompra.valCompra_fecVigenFin = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(strFecFin));
                            _valeCompra.valCompra_ListDetalle = lisDet;
                            _valeCompra.valCompra_total = strTotal;


                        }
                    }
                }
            }
            catch (Exception)
            {

                _valeCompra = null;
            }
            return _valeCompra;
        }

        public Boolean InsertarGeneracionValeCompra()
        {
            string sqlquery = "USP_Insertar_GeneracionValeCompra";
            Boolean valida = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@valCom_id", 0);
                        cmd.Parameters.AddWithValue("@valCom_Clienteid", valeCompra.valCliente_id);
                        cmd.Parameters.AddWithValue("@valCom_Comcepto", valeCompra.valCompra_descripcion);
                        cmd.Parameters.AddWithValue("@valCom_Fecini", valeCompra.valCompra_fecVigenFin);
                        cmd.Parameters.AddWithValue("@valCom_Fecfin", valeCompra.valCompra_fecVigenFin);
                        cmd.Parameters.AddWithValue("@valCom_listDetalle", valeCompra.valCompra_strListDetalle);
                        cmd.Parameters.AddWithValue("@valCom_UsuCrea", "jmallqui");
                        cmd.Parameters.AddWithValue("@valCom_fecCrea", valeCompra.valCompra_fecVigenFin);
                        cmd.Parameters.AddWithValue("@valCom_IpCrea", "11");
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


        public Ent_ValeCompra InsertarGeneracionValeCompraEntidad()
        {

            Ent_ValeCompra _valeCompra = null;
            List<Ent_ValeCompraDetalle> lisDet = null;
            string strValCompId = "";
            string strRuc = "";
            string strRazon = "";
            string strCodigoCli = "";
            string strDireccionCli = "";
            string strComcepto = "";
            string strFecIni = "";
            string strFecFin = "";
            string sqlquery = "USP_Insertar_GeneracionValeCompra";
           
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure; 


                        cmd.Parameters.AddWithValue("@valCom_ClienteRuc", valeCompra.valCompra_ruc);
                        cmd.Parameters.AddWithValue("@valCom_total", valeCompra.valCompra_total);
                        cmd.Parameters.AddWithValue("@valCom_Comcepto", valeCompra.valCompra_descripcion);
                        cmd.Parameters.AddWithValue("@valCom_Fecini", valeCompra.valCompra_fecVigenIni);
                        cmd.Parameters.AddWithValue("@valCom_Fecfin", valeCompra.valCompra_fecVigenFin);
                        cmd.Parameters.AddWithValue("@valCom_listDetalle", valeCompra.valCompra_strListDetalle);
                        cmd.Parameters.AddWithValue("@valCom_UsuCrea", valeCompra.valCompra_usuCreacion);
                        cmd.Parameters.AddWithValue("@valCom_fecCrea", valeCompra.valCompra_fecVigenFin);
                        cmd.Parameters.AddWithValue("@valCom_IpCrea", valeCompra.valCompra_ipCreacion);
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            _valeCompra = new Ent_ValeCompra();
                            lisDet = new List<Ent_ValeCompraDetalle>();
                            
                            while (dr.Read())
                            {
                                Ent_ValeCompraDetalle _valeCompraDetalle = new Ent_ValeCompraDetalle();

                                strValCompId = dr["VCom_ID"].ToString();
                                strRuc = dr["VCom_Ruc"].ToString();
                                strCodigoCli = dr["VCom_CliCodigo"].ToString();
                                strRazon = dr["VCom_Razon"].ToString();
                                strComcepto = dr["VCom_Descripcion"].ToString();
                                strFecIni = dr["VCom_FecIniVigencia"].ToString();
                                strFecFin = dr["VCom_FecFinVigencia"].ToString();
                                strDireccionCli = dr["VCom_CliDireccion"].ToString();

                                _valeCompraDetalle.valCom_codeBarra = dr["Barra"].ToString();
                                _valeCompraDetalle.valComDet_montoLetras = dr["MontLetras"].ToString();
                                _valeCompraDetalle.valComDet_correlativo = dr["Correlativo"].ToString();
                                _valeCompraDetalle.valComDet_monto = Convert.ToDecimal(dr["VComDet_Monto"].ToString());
                                _valeCompraDetalle.valComDet_cantidad = Int32.Parse(dr["VComDet_Cantidad"].ToString());

                                lisDet.Add(_valeCompraDetalle);

                            }

                            _valeCompra.valCompra_id = Int32.Parse(strValCompId);
                            _valeCompra.valCompra_ruc = strRuc;
                            _valeCompra.valCompra_razon = strRazon;
                            _valeCompra.cli_codigo = strCodigoCli;
                            _valeCompra.cli_Direccion = strDireccionCli;
                            _valeCompra.valCompra_descripcion = strComcepto;
                            _valeCompra.valCompra_fecVigenIni = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(strFecIni));
                            _valeCompra.valCompra_fecVigenFin = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(strFecFin));
                            _valeCompra.valCompra_ListDetalle = lisDet;

                        }

                    }

                }

            }
            catch (Exception exc)
            {
                _valeCompra = null;
            }
            return _valeCompra;
        }

        public Ent_ValeCompra listarCupones(string strId)
        {
            Ent_ValeCompra _valeCompra = null;
            List<Ent_ValeCompraDetalle> lisDet = null;

            string strValCompId = "";
            string strRuc = "";
            string strRazon = "";
            string strCodigoCli = "";
            string strDireccionCli = "";
            string strComcepto = "";
            string strFecIni = "";
            string strFecFin = "";

            string sqlquery = "USP_obtener_Cupones_ValesCompra";
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VCom_ID", strId);
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            _valeCompra = new Ent_ValeCompra();
                            lisDet = new List<Ent_ValeCompraDetalle>();

                            while (dr.Read())
                            {
                                Ent_ValeCompraDetalle _valeCompraDetalle = new Ent_ValeCompraDetalle();

                                strValCompId = dr["VCom_ID"].ToString();
                                strRuc = dr["VCom_Ruc"].ToString();
                                strCodigoCli = dr["VCom_CliCodigo"].ToString();
                                strRazon = dr["VCom_Razon"].ToString();
                                strComcepto = dr["VCom_Descripcion"].ToString();
                                strFecIni = dr["VCom_FecIniVigencia"].ToString();
                                strFecFin = dr["VCom_FecFinVigencia"].ToString();
                                strDireccionCli = dr["VCom_CliDireccion"].ToString();

                                _valeCompraDetalle.valCom_codeBarra = dr["Barra"].ToString();
                                _valeCompraDetalle.valComDet_montoLetras = dr["MontLetras"].ToString();
                                _valeCompraDetalle.valComDet_correlativo = dr["Correlativo"].ToString();
                                _valeCompraDetalle.valComDet_monto = Convert.ToDecimal(dr["VComDet_Monto"].ToString());


                                lisDet.Add(_valeCompraDetalle);

                            }

                            _valeCompra.valCompra_id = Int32.Parse(strValCompId);
                            _valeCompra.valCompra_ruc = strRuc;
                            _valeCompra.valCompra_razon = strRazon;
                            _valeCompra.cli_codigo = strCodigoCli;
                            _valeCompra.cli_Direccion = strDireccionCli;
                            _valeCompra.valCompra_descripcion = strComcepto;
                            _valeCompra.valCompra_fecVigenIni = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(strFecIni));
                            _valeCompra.valCompra_fecVigenFin = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(strFecFin));
                            _valeCompra.valCompra_ListDetalle = lisDet;

                        }
                    }
                }
            }
            catch (Exception)
            {

                _valeCompra = null;
            }
            return _valeCompra;
        }


        public Boolean ActualizarValeCompra()
        {

            string sqlquery = "USP_Actualizar_ValeCompra";
            Boolean valida = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(Ent_Conexion.conexion))
                {
                    if (cn.State == 0) cn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlquery, cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VCom_Generado", valeCompra.valCompra_generado);
                        cmd.Parameters.AddWithValue("@valCom_id", valeCompra.valCompra_id);
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
