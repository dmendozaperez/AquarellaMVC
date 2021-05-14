using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.RRHH
{
    public class Ent_Promotor_Lider
    {
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string Promotor { get; set; }
        public string Documento { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
        public string Fecing { get; set; }
        public DateTime? Fecactv { get; set; }
        public string Fec_Nac { get; set; }
        public string Zona { get; set; }
        public string Activo { get; set; }
        //Campos Adicionales
        public string Bas_Id { get; set; }
        public string Are_Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
    public class Ent_KPI_Asesor
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
    public class Ent_KPI_Lider
    {
        public string IdAsesor { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
    public class Ent_ConsultaKPI_Detalle
    {
        public string Lider { get; set; }
        public string Asesor { get; set; }
        public int? Anio { get; set; }
        public string Mes { get; set; }
        public Decimal? Facturacion { get; set; }
        public int? Continuas { get; set; }
        public int? Afiliadas { get; set; }
        public int? Reactivadas { get; set; }
        public int? Activasenmes { get; set; }
        public int? Desactivadas { get; set; }
        public Decimal? PorDesact { get; set; }
        public Decimal? Reg_Mes { get; set; }
        public Decimal? TactRegMes { get; set; }
        public Decimal? PorAfiliadasMes { get; set; }
        public int? ActivasOtroMes { get; set; }
        public int? TotalActivas { get; set; }
        public Decimal? TicketProm { get; set; }
        //Campos de busqueda
        public string IdAsesor { get; set; }
        public string IdLider { get; set; }
        public bool IsAseOrLider { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class Ent_ConsultaKPI
    {
        public string concepto { get; set; }
        public Decimal Enero { get; set; }
        public Decimal Febrero { get; set; }
        public Decimal Marzo { get; set; }
        public Decimal Abril { get; set; }
        public Decimal Mayo { get; set; }
        public Decimal Junio { get; set; }
        public Decimal Julio { get; set; }
        public Decimal Agosto { get; set; }
        public Decimal Septiembre { get; set; }
        public Decimal Octubre { get; set; }
        public Decimal Noviembre { get; set; }
        public Decimal Diciembre { get; set; }
        //Campos de busqueda
        public string IdAsesor { get; set; }
        public string IdLider { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
