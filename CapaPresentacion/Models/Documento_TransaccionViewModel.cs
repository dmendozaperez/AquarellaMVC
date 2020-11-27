using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models
{
    public class Documento_TransaccionViewModel
    {
        public string Doc_Tra_Id { get; set; }
        public double Doc_Tra_BasId { get; set; }
        public string Doc_Tra_ConId { get; set; }
        public string Doc_Tra_Doc { get; set; }
        public double Doc_Tra_SubTotal { get; set; }
        public string Doc_Tra_GruId { get; set; }
        public string Doc_Tra_Comentario { get; set; }
        public double Doc_Tra_Igv { get; set; }
        public double Doc_Tra_PercepcionM { get; set; }
        public double Doc_Tra_PercepcionP { get; set; }
        public string Doc_Tra_Fecha { get; set; }
        public double Doc_Tra_UsuCreacion { get; set; }
        public double Doc_Tra_IgvPorc { get; set; }
        public string Doc_Tra_Anticipo { get; set; }
        public string Doc_Tra_Fecha_Act { get; set; }
        public string Doc_Tra_Finanza { get; set; }
        public string Doc_Tra_AlmID { get; set; }
        public string Doc_Bin_Ser { get; set; }
        public string Doc_Bin_Cod { get; set; }
        public string Doc_Bin_NumTar { get; set; }
        public string Doc_Grupo { get; set; }
    }
    public class Lista_Cuenta_ContablesViewModel
    {
        public string Clear_id { get; set; }
        public string Cuenta { get; set; }
        public string CuentaDes { get; set; }
        public string TipoEntidad { get; set; }
        public string CodigoEntidad { get; set; }
        public string DesEntidad { get; set; }
        public string Tipo { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public DateTime? Fecha { get; set; }
        public double? Debe { get; set; }
        public double? Haber { get; set; }
        public string devito { get; set; }
        public double? Amount { get; set; }
        public string Concepto { get; set; }
        public int? Ad_Co { get; set; }
        public DateTime? Pad_Pay_Date { get; set; }
        public int? Contador { get; set; }
        //Campos adicionales
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int IdCliente { get; set; }
    }
}