using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Documento_Transaccion
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

    public class Ent_Lista_Cuenta_Contables
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
        public Decimal? Debe { get; set; }
        public Decimal? Haber { get; set; }
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

    public class Ent_Listar_Cliente_Banco
    {
        public string Ban_Id { get; set; }
        public string Campo { get; set; }
    }

    public class Ent_Venta_Semanal
    {
        public int? Id { get; set; }
        public string Dtv_Clear { get; set; }
        public string Promotor { get; set; }
        public string DniRuc { get; set; }
        public string Ped { get; set; }
        public string BolFact { get; set; }
        public string FechaDoc { get; set; }
        public Decimal? MontoFac { get; set; }
        public string NroVouBcp { get; set; }
        public string FechavouBcp { get; set; }
        public Decimal? MontoVouBcp { get; set; }
        public string NroVisa { get; set; }
        public string FechaVisa { get; set; }
        public Decimal? MontoVisa { get; set; }
        public string Nronc { get; set; }
        public string Fechanc { get; set; }
        public Decimal? Montonc { get; set; }
        public string FechaSaldoant { get; set; }
        public Decimal? MontoSaldoant { get; set; }
        public Decimal? TotalPagos { get; set; }
        public Decimal? SaldoFavor { get; set; }
        //Campos adicionales
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
    public class Ent_Saldos_Anticipos{
        public string Documento { get; set; }
        public string Cliente { get; set; }
        public Decimal? Saldo { get; set; }
        public string SerieFac { get; set; }
        public string NumeroFac { get; set; }
        public string Fec_Fac { get; set; }
        public Decimal? MontoFac { get; set; }
        public string SerieNc { get; set; }
        public string NumeroNc { get; set; }
        public string Fec_Nc { get; set; }
        public Decimal? MontoNc { get; set; }
        public Decimal? Monto_Util { get; set; }
        public Decimal? Percepcion { get; set; }
        public bool? Chk { get; set; }
        public int? Bas_Id { get; set; }
        //campos adicionales
        public decimal usu_ingreso { get; set; }
    }

    public class Ent_Validar_Pagos
    {
        public int NumBanco { get; set; }
        public int NumTipoArchivo { get; set; }
        public string NumDocuemnto { get; set; }
        public string NumPedido { get; set; }
        public string FecOperacion { get; set; }
        public string DesOperacion { get; set; }
        public string NumOperacion { get; set; }
        public string MonOperacion { get; set; }
        public string NomArchivo { get; set; }
        public Decimal Usu_Validar { get; set; }
    }

}
