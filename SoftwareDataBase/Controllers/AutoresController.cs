using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoftwareDataBase.Models;

namespace SoftwareDataBase.Controllers
{
    public class AutoresController : Controller
    {
        private BibliotecaOKEntities db = new BibliotecaOKEntities();

        // GET: Autores
        public ActionResult Index()
        {
            var autores = db.Autores.Include(a => a.Pais);
            return View(autores.ToList());
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
    }
}
