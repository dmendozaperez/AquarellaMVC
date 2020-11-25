using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models
{
    public class ArticuloViewModel
    {
        public string IdArticulo { get; set; }
        public string Cat_Principal { get; set; }
        public string SubCategoria { get; set; }
        public string Marca { get; set; }
        public string Descripcion { get; set; }
        public Decimal PrecioIgv { get; set; }
        public Decimal PrecioSinIgv { get; set; }
        public Decimal Costo { get; set; }
    }
}