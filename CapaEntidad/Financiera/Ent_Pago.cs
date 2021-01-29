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
}
