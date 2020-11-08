using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Asn1.Ocsp;
using SoftwareDataBase.Models;

namespace SoftwareDataBase.Controllers
{
    public class AutoresController : Controller
    {
        private BibliotecaOKEntities db = new BibliotecaOKEntities();

        // GET: Autores
        public ActionResult Index(string buscar)
        {
            #region Otra Opcion
            //var autores = db.Autores.Include(a => a.Pais);
            #endregion
            #region Otra Opcion
            //IEnumerable<Models.Autore> autores = from x in db.Autores
            //                                     select x;
            //autores = autores.Where(x => x.IdPais == 1);
            #endregion
            #region Otra Opcion
            //IQueryable<Models.Autore> autores = from x in db.Autores
            //                                    select x;
            //return View(autores.ToList());
            #endregion
            #region Filtrado con TextBox
            var autores = from x in db.Autores
                          select x;
            //El IQuereably, todavia nose ejecuta y podemos preguntar si el STRING es Null primero o vacio
            //IQuereably es el Mejor tipo
            //IEnumerable , carga todo en memoria, por eso gasta mas recurso
            if (!string.IsNullOrEmpty(buscar))
            {
                //Buscamos/Filtramos
                autores = autores.Where(x => x.Apellido.Contains(buscar));
                //Si queremos que comience con las letras que estamos tipiando seria:
                autores = autores.Where(x => x.Apellido.StartsWith(buscar));
            }
            return View(autores.ToList());//En primera instancia trae todos los autores, hasta que envio un dato
            #endregion



        }

        // GET: Autores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autore autore = db.Autores.Find(id);
            if (autore == null)
            {
                return HttpNotFound();
            }
            return View(autore);
        }

        // GET: Autores/Create
        public ActionResult Create()
        {
            ViewBag.IdPais = new SelectList(db.Paises, "ID", "Nombre");
            return View();
        }

        // POST: Autores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Apellido,Nombre,FechaNacimiento,IdPais")] Autore autore)
        {
            if (ModelState.IsValid)
            {
                db.Autores.Add(autore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdPais = new SelectList(db.Paises, "ID", "Nombre", autore.IdPais);
            return View(autore);
        }

        // GET: Autores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autore autore = db.Autores.Find(id);
            if (autore == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPais = new SelectList(db.Paises, "ID", "Nombre", autore.IdPais);
            return View(autore);
        }

        // POST: Autores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Apellido,Nombre,FechaNacimiento,IdPais")] Autore autore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autore).State = EntityState.Modified;
                db.SaveChanges();
                #region Otra Opcion para Trabajar con el StoreProcedure es en el Contralador
                /*
                if(ModelState.IsValid){
                
                _context.MyStoreProcedure(parametros del Procedure);
                
                return RedirecToAction("Index");
                }
                Sino--
                ViewBag.Nacionalida = new SelecList(_context.Paises, "ID", "Descripcion", autore.Nacionalidad)
                return View(autore);

                */
                #endregion


                return RedirectToAction("Index");
            }
            ViewBag.IdPais = new SelectList(db.Paises, "ID", "Nombre", autore.IdPais);
            return View(autore);
        }

        // GET: Autores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autore autore = db.Autores.Find(id);
            if (autore == null)
            {
                return HttpNotFound();
            }
            return View(autore);
        }

        // POST: Autores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Autore autore = db.Autores.Find(id);
            db.Autores.Remove(autore);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public FileStreamResult ExportaPDF()
        {
            var autores = db.Autores.ToList();

            //Creamos un WebGrid
            WebGrid grid = new WebGrid(source:autores, canPage:false, canSort:false);
            string gridHTML =
                    grid.GetHtml(
                        columns: grid.Columns
                                (
                                     grid.Column("ID", header: "Codigo"),
                                     grid.Column("Apellido", header: "Apellido"),
                                     grid.Column("Nombre", header: "Nombre"),
                                     grid.Column("IdPais", header: "Cod.Pais"),
                                     grid.Column("Pais.Nombre", header: "Pais")
                                )


                        ).ToString();
            //--------------------
            string exportaData = string.Format("<html><head>{0}</head><body>{1}</body></html>","<p>Lista de Autores</p> <style>table{ borde-spacing:10px; border-collapse:separate;}</style>", gridHTML);

            var bytes = System.Text.Encoding.UTF8.GetBytes(exportaData);
            
            using ( var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                //Setiamos el tamalo y margenes
                var document = new iTextSharp.text.Document(PageSize.A4,50,50,50,50);
                var writer = PdfWriter.GetInstance(document,output);
                writer.CloseStream = false;
                
                document.Open();

                var xmlWorker = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();
                xmlWorker.ParseXHtml(writer,document,input,System.Text.Encoding.UTF8);

                document.Close();
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
                
        }


        public ActionResult ExportaExcel()
        {
            var grid = new GridView();
            grid.DataSource = db.Autores.ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "inline; filename=Autores.xls");
            Response.ContentType = "application/excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            //Renderizamos
            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();

            return View("Index");
        }
    }
}
