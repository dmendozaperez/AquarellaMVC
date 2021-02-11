using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Facturacion
{
    public class Ent_Movimientos_Ventas
    {
        public string Mcv_Description { get; set; }
        public int? Anno { get; set; }
        public int? Can_Week_No { get; set; }
        public Decimal Ventas { get; set; }
        public Decimal Podv { get; set; }
        public Decimal Pventas { get; set; }
        public Decimal Pventasneto { get; set; }
        public Decimal Pmargen { get; set; }
        public Decimal Pmargenpor { get; set; }

        //campo adicionales de buscqueda
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string IdTipoArticulo { get; set; }
    }
    public class Ent_Movimientos_Ventas_Chart
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string borderWidth { get; set; }
        public decimal[] data { get; set; }
    }
    public class Ent_Movimientos_Ventas_Chart_Data
    {
        public string[] labels { get; set; }
        public string[] labelsTooltip { get; set; }
        public List<Ent_Movimientos_Ventas_Chart> datasets { get; set; }
    }

    public class Ent_Comisiones
    {
        public int AreaId { get; set; }
        public string Asesor { get; set; }
        public string Lider { get; set; }
        public string LiderDni { get; set; }
        public Decimal? TotPares { get; set; }
        public Decimal? TotVenta { get; set; }
        public Decimal? PorComision { get; set; }
        public Decimal? Comision { get; set; }
        public Decimal? Bonosnuevas { get; set; }
        public Decimal? SubTotalSinIGV { get; set; }
        //campos adicionales de busqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
    public class Ent_Resumen_Ventas
    {
        public int? Anno { get; set; }
        public int? Semana { get; set; }
        public int? TotalTickets { get; set; }
        public int? Pares { get; set; }
        public Decimal? TotalIgv { get; set; }
        public Decimal? PrecioPromedio { get; set; }
        public Decimal? NParesTicket { get; set; }
        public int? Anno1 { get; set; }
        public int? Semana1 { get; set; }
        public int? TotalTickets1 { get; set; }
        public int? Pares1 { get; set; }
        public Decimal? TotalIgv1 { get; set; }
        public Decimal? PrecioPromedio1 { get; set; }
        public Decimal? NParesTicket1 { get; set; }
        //Campos condicionales de busqueda
        public int Codigo { get; set; }
        public int Descripcion { get; set; }
    }

    public class Ent_Lider_Ventas
    {
        public string lider { get; set; }
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
        public Decimal Grand_Total {get; set;}
        //campos adicionales de busqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
