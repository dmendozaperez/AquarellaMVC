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
}
