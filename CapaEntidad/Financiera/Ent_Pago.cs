using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Financiera
{
    public class Ent_Pago
    {
        public string Pag_Id { get; set; }
        public double Pag_BasId { get; set; }
        public string Pag_BanId { get; set; }
        public string Pag_Num_Consignacion { get; set; }
        public string Pag_Num_ConsFecha { get; set; }
        public string Pag_Fecha_Creacion { get; set; }
        public double Pag_Monto { get; set; }
        public string Pag_Comentario { get; set; }
        public string Pag_EstId { get; set; }
        public string Pag_Fecha_Evalua { get; set; }
        public string Pag_ConId { get; set; }
        public string Pag_Num_Tarjeta { get; set; }
        public double Pag_Usu_Creacion { get; set; }
        public string Pag_Pedido { get; set; }
        //campos aumentados
        public int Existe { get; set; }
        public int RetVal { get; set; }
    }

    public class Ent_Listar_Cliente_Pagos
    {
        public int PagoId { get; set; }
        public string Documento { get; set; }
        public string NombreCompleto { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimeroApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Correo { get; set; }
        public string NumeroConsignacion { get; set; }
        public string FechaConsignacion { get; set; }
        public string FechaCreacion { get; set; }
        public Decimal Monto { get; set; }
        public string Estado { get; set; }
        public string EstadoNombre { get; set; }
        //Campos Aumentados
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public int IdCliente { get; set; }
    }
    public class Ent_Listar_Verificar_Pagos
    {
        public string Pag_Id { get; set; }
        public string Lider { get; set; }
        public string Bas_Documento { get; set; }
        public string Promotor { get; set; }
        public string Ban_Descripcion { get; set; }
        public string Pag_Num_Consignacion { get; set; }
        public string Con_Descripcion { get; set; }
        public string Pag_Num_ConsFecha { get; set; }
        public Decimal Pag_Monto { get; set; }
        public string Est_Id { get; set; }
        public string Con_Id { get; set; }
        public string Are_Id { get; set; }
    }

    public class Ent_Operacion_Gratuita
    {
        public string Tipo { get; set; }
        public string Fecha { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string Doc_cliente { get; set; }
        public string Cliente { get; set; }
        public string EstadoDescripcion { get; set; }
        public Decimal SubTotal { get; set; }
        public Decimal IGV { get; set; }
        public Decimal Total { get; set; }
        //Campos adicionales
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string TipoNombre { get; set; }
    }
    public class Ent_Saldo_Cliente
    {
        public string Asesor { get; set; }
        public string Dniruc { get; set; }
        public string Lider { get; set; }
        public string Cliente { get; set; }
        public string Concepto { get; set; }
        public string Documento { get; set; }
        public DateTime? Fecha_Transac { get; set; }
        public DateTime? Fecha_Doc { get; set; }
        public Decimal? Monto { get; set; }
        public string Valida { get; set; }
        //Campos Adicionales
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Bas_Id { get; set; }
        public string Cod_Id { get; set; }
        public string Usu_Tipo { get; set; }
        public string Bas_Aco_Id { get; set; }
    }
    public class Ent_Movimientos_Pagos
    {
        public string Pag_Id { get; set; }
        public string Fecha_Op { get; set; }
        public string Des_Operacion { get; set; }
        public Decimal? Op_Monto { get; set; }
        public string Op_Numero { get; set; }
        public string Fecha_Op2 { get; set; }
        public string Dni_Ruc { get; set; }
        public string Cliente { get; set; }
        public string Fecha_Doc { get; set; }
        public string Num_Doc { get; set; }
        public Decimal? Importe_Doc { get; set; }
        public string Fecha_Ncredito { get; set; }
        public string Num_Ncredito { get; set; }
        public Decimal? Importe_Ncredito { get; set; }
        public string Fecha_Ncredito2 { get; set; }
        public string Num_Ncredito2 { get; set; }
        public Decimal? Importe_Ncredito2 { get; set; }
        public string Fecha_Ncredito3 { get; set; }
        public string Num_Ncredito3 { get; set; }
        public Decimal? Importe_Ncredito3 { get; set; }
        public string Fecha_Ncredito4 { get; set; }
        public string Num_Ncredito4 { get; set; }
        public Decimal? Importe_Ncredito4 { get; set; }
        public string Fecha_Ncredito5 { get; set; }
        public string Num_Ncredito5 { get; set; }
        public Decimal? Importe_Ncredito5 { get; set; }
        public string Fecha_Ncredito6 { get; set; }
        public string Num_Ncredito6 { get; set; }
        public Decimal? Importe_Ncredito6 { get; set; }
        public string Fecha_Ncredito7 { get; set; }
        public string Num_Ncredito7 { get; set; }
        public Decimal? Importe_Ncredito7 { get; set; }
        public Decimal? Base_Imponible { get; set; }
        public Decimal? Percepcion { get; set; }
        public Decimal? Total { get; set; }
        public string Fecha_Saldo_Ant { get; set; }
        public Decimal? Importe_Saldo_Ant { get; set; }
        public Decimal? Pagar { get; set; }
        public Decimal? Deposito { get; set; }
        public Decimal? Saldo_Favor { get; set; }
        public Decimal? Ajuste { get; set; }
        public Decimal? Items_Mov { get; set; }
        public string Grupo { get; set; }
        //campo adicionales de buscqueda
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
