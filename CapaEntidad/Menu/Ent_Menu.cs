using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Menu
{
    public class Ent_Menu
    {
        #region<REGION DE VARIABLES>

        public Int32 fun_id { get; set; }
        public string fun_nombre { get; set; }     
        public Int32 fun_padre { get; set; }
     
        public Int32 fun_orden { get; set; }
        public Int32 apl_id { get; set; }

        public string apl_controller { get; set; }
        public string apl_action { get; set; }
        #endregion
    }
    public class Ent_Menu_Items
    {
        public int Id { get; set; }
        public string nameOption { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string area { get; set; }
        public string imageClass { get; set; }
        public string activeli { get; set; }
        public bool estatus { get; set; }
        public int parentId { get; set; }
        public bool isParent { get; set; }
        public bool hasChild { get; set; }
    }

}
