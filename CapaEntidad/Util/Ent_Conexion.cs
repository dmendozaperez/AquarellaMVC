using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Util
{
    public class Ent_Conexion
    {
        public static string conexion
        {
            get {
                // return "Server=des.bgr.pe;Database=BdTiendaReplica;User ID=junior;Password=Bata2018**@;Trusted_Connection=False;";
                return "Server=10.10.10.206;Database=BdAquarellaQA2;User ID=sa;Password=Bata2013;Trusted_Connection=False;";
                //return "Server=posperu.bgr.pe;Database=BDPOS;User ID=pos_oracle;Password=Bata2018**;Trusted_Connection=False;";

            }
        }

        public static string conexionPosPeru
        {
            get
            {
                // return "Server=des.bgr.pe;Database=BdTiendaReplica;User ID=junior;Password=Bata2018**@;Trusted_Connection=False;";
                return "Server=posperu.bgr.pe;Database=BDPOS;User ID=pos_oracle;Password=Bata2018**;Trusted_Connection=False;";

            }
        }
        public static string strDirectorio
        {
            get
            {

                return "~/Cupones/Bata_"; ;

            }
        }

        public static string plantilla
        {
            get
            {
            
                return "~/FormatoCupon/htmlcupon20.html";

            }
        }

        public static string servidorReporte
        {
            get
            {
                //return "http://posperu.bgr.pe:80/BataRptSrv/";
                return "http://posperu/BataRptSrv/";
            }
        }

        public static string usuarioReporte
        {
            get
            {
                return "ReportBata";
            }
        }

        public static string passwordReporte
        {
            get
            {
                return "Bata2018**";
            }
        }

        public static string dominioReporte
        {
            get
            {
                return "BataRptSrv";
            }
        }

        public static string CarpetaPlanillaReporte
        {
            get
            {
                return "ReportBata";
            }
        }


    }
}
