using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapaPresentacion.Models.Control
{
    public class LoginViewModel
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public bool Recordar { get; set; }
        public string returnUrl { get; set; }
    }
}