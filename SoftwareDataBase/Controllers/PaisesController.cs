using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftwareDataBase.Controllers
{
    public class PaisesController : Controller
    {
        Models.BibliotecaOKEntities _context = new Models.BibliotecaOKEntities();
        // GET: Paises
        public ActionResult Index()
        {
            //Linq creamos una lista de paise
            var listaPaises = (from x in _context.Paises
                              select x);

            return View("Index",listaPaises.ToList());
        }
    }
}