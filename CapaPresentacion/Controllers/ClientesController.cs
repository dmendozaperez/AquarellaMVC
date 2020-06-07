using CapaDato.Cliente;
using CapaDato.Maestros;
using CapaEntidad.Cliente;
using CapaEntidad.General;
using CapaEntidad.Maestros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacion.Controllers
{
    public class ClientesController : Controller
    {
        // GET: Clientes

        private Dat_Cliente dat_cliente = new Dat_Cliente();
        private string _session_listCliente_private = "_session_listCliente_private";
        public ActionResult Index()
        {

            ViewBag.lider = dat_cliente.lista_lider();
           // Session[_session_listCliente_private] = dat_cliente.lista_cliente();

            return View();
        }
        public ActionResult getListClienteAjax(Ent_jQueryDataTableParams param)
        {

            List<Ent_Cliente_Lista> listcliente = new List<Ent_Cliente_Lista>();
            /*verificar si esta null*/
            if (Session[_session_listCliente_private] == null)
            {
                listcliente = new List<Ent_Cliente_Lista>();
                listcliente = lista(); //datOE.get_lista_atributos();
                if (listcliente == null)
                {
                    listcliente = new List<Ent_Cliente_Lista>();
                }
                Session[_session_listCliente_private] = listcliente;
            }

            //Traer registros
            IQueryable<Ent_Cliente_Lista> membercol = ((List<Ent_Cliente_Lista>)(Session[_session_listCliente_private])).AsQueryable();  //lista().AsQueryable();

            //Manejador de filtros
            int totalCount = membercol.Count();
            IEnumerable<Ent_Cliente_Lista> filteredMembers = membercol;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredMembers = membercol
                    .Where(m => m.dni.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.nombres.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.correo.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.telefono.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.celular.ToUpper().Contains(param.sSearch.ToUpper()) ||
                     m.direccion.ToUpper().Contains(param.sSearch.ToUpper()) 
                     );
            }
            //Manejador de orden
            var sortIdx = Convert.ToInt32(Request["iSortCol_0"]);

            if (param.iSortingCols > 0)
            {
                if (Request["sSortDir_0"].ToString() == "asc")
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderBy(o => o.dni); break;
                        case 1: filteredMembers = filteredMembers.OrderBy(o => o.nombres); break;
                        case 2: filteredMembers = filteredMembers.OrderBy(o => o.correo); break;                        
                    }
                }
                else
                {
                    switch (sortIdx)
                    {
                        case 0: filteredMembers = filteredMembers.OrderByDescending(o => o.dni); break;
                        case 1: filteredMembers = filteredMembers.OrderByDescending(o => o.nombres); break;
                        case 2: filteredMembers = filteredMembers.OrderByDescending(o => o.correo); break;                        
                    }
                }
            }
            var displayMembers = filteredMembers
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);
            var result = from a in displayMembers
                         select new
                         {
                             a.dni,
                             a.nombres,
                             a.correo,
                             a.telefono,
                             a.celular,
                             a.direccion,
                         };
            //Se devuelven los resultados por json
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = filteredMembers.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        //public PartialViewResult lista_clientes()
        //{
        //    return PartialView(lista());
        //}
        public List<Ent_Cliente_Lista> lista()
        {

            List<Ent_Cliente_Lista> listcliente = dat_cliente.lista_cliente();
            Session[_session_listCliente_private] = listcliente;
            return listcliente;
        }

        public ActionResult ClienteEditar(string estado)
        {

            string actionName = this.ControllerContext.RouteData.GetRequiredString("action");
            string controllerName = this.ControllerContext.RouteData.GetRequiredString("controller");
            string return_view = actionName + "|" + controllerName;

            if (estado==null) return RedirectToAction("Index", "Clientes");

            ViewBag.Estado = estado;
            ViewBag.EstadoDes = (estado == "1" ? "Creando nuevo Cliente" : "Modificando Cliente");

           Dat_Usuario_Tipo dat_usu_tipo = new Dat_Usuario_Tipo();
           ViewBag.UsuTipo = dat_usu_tipo.get_lista(true);

            Dat_Documento_Tipo dat_doc_tipo = new Dat_Documento_Tipo();
            ViewBag.DocTipo = dat_doc_tipo.get_lista();

            Dat_Persona_Tipo dat_per_tipo = new Dat_Persona_Tipo();
            ViewBag.PerTipo = dat_per_tipo.get_lista();


            Dat_Lugar dat_lugar = new Dat_Lugar();

            List<Ent_Lugar> combo_dep_prv_dis = dat_lugar.get_lista();

            ViewBag.Dep = combo_departamento(combo_dep_prv_dis);

            List<Ent_Lugar> lista_prov = new List<Ent_Lugar>();

            ViewBag.Prov = lista_prov;

            List<Ent_Lugar> lista_dis = new List<Ent_Lugar>();

            ViewBag.Dis = lista_dis;

            ViewBag.DepProvDis = combo_dep_prv_dis;

            Dat_Combo_Lider cbolider = new Dat_Combo_Lider();

            ViewBag.Lider = cbolider.lista_lider();

            Ent_Cliente cliente = new Ent_Cliente();

            ViewBag.cliente = cliente;

            return View();
        }
        private List<Ent_Lugar> combo_departamento(List<Ent_Lugar> combo_general)
        {
            List<Ent_Lugar> listar = null;
            try
            {
                listar = new List<Ent_Lugar>();
                listar = (from grouping in combo_general.GroupBy(x => new Tuple<string, string>(x.dep_id, x.dep_descripcion))
                          select new Ent_Lugar
                          {
                              dep_id = grouping.Key.Item1,
                              dep_descripcion = grouping.Key.Item2,
                          }).OrderBy(a => a.dep_descripcion).ToList();
            }
            catch
            {


            }
            return listar;
        }

        public ActionResult valida_cliente(string dni,string correo)
        {
            string mensaje = "";
            try
            {
                /*si trare valor 0 entonces no hay concidencias*/
               string valida = dat_cliente.valida_cliente(dni, correo);
               switch(valida)
                {                   
                    case "1":
                        mensaje = "El Numero de documento existe, ingrese otro numero";
                        break;
                    case "2":
                        mensaje = "El correo existe, ingrese otro correo";
                        break;
                    case "3":
                        mensaje = "Error de conexion";
                        break;
                }


                return Json(new { estado = valida, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { estado = "3", mensaje = ex.Message });
            }


        }

        public ActionResult GuardarCliente(Ent_Cliente dataArray)
        {
            try
            {

                string grabar= dat_cliente.grabar_clientes(1, 1, dataArray);

                if (grabar.Length==0)
                {
                    return Json(new { estado = "0", mensaje = "Se Guardo con exito." });
                }
                else
                {
                    return Json(new { estado = "1", mensaje = grabar});
                }
                
            }
            catch (Exception exc)
            {

                return Json(new { estado = "1", mensaje = exc.Message });
            }
            
        }
    }
}