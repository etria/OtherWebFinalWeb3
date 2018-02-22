using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameStoreDB1;

namespace GameStoreDB1.Controllers
{
    public class ConsolesController : Controller
    {
        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();

        // GET: Consoles
        public ActionResult Index()
        {
            var consoles = db.Consoles.Include(c => c.Color1).Include(c => c.Model).OrderBy(c=>c.Model.GameSystem.SystemName);
            return View(consoles.ToList());
        }

        public ActionResult AddItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Console console = db.Consoles.Find(id);
            if (console == null)
            {
                return HttpNotFound();
            }


            return RedirectToAction("ConsoleAddItem", "AddingItems", new { id = console.ConsoleId, type = "Console" });
        }
        // GET: Consoles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Console console = db.Consoles.Find(id);
            if (console == null)
            {
                return HttpNotFound();
            }
            return View(console);
        }

        // GET: Consoles/Create
        public ActionResult Create()
        {
            var mod = db.Models.Include(m => m.GameSystem).OrderBy(m => m.GameSystem.SystemName).ThenBy(m => m.ModelName);
            ViewBag.Color = new SelectList(db.Colors, "ColorId", "Color1");
            ViewBag.ModelID = new SelectList(mod, "ModelId", "ModelName");
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            return View();
        }

        // POST: Consoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ConsoleId,ModelID,SerialNumber,Color")] Console console, int ConditionId)
        {
            if (ModelState.IsValid)
            {
                Item item = new Item();
                item.TypeId = 3;
                item.ConditionId = ConditionId;
                db.Items.Add(item);
                db.SaveChanges();
                console.Items.Add(item);
                db.Consoles.Add(console);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Color = new SelectList(db.Colors, "ColorId", "Color1", console.Color);
            ViewBag.ModelID = new SelectList(db.Models, "ModelId", "ModelName", console.ModelID);
            return View(console);
        }

        // GET: Consoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Console console = db.Consoles.Find(id);
            if (console == null)
            {
                return HttpNotFound();
            }
            ViewBag.Color = new SelectList(db.Colors, "ColorId", "Color1", console.Color);
            ViewBag.ModelID = new SelectList(db.Models, "ModelId", "ModelName", console.ModelID);
            return View(console);
        }

        // POST: Consoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConsoleId,ModelID,SerialNumber,Color")] Console console)
        {
            if (ModelState.IsValid)
            {
                db.Entry(console).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Color = new SelectList(db.Colors, "ColorId", "Color1", console.Color);
            ViewBag.ModelID = new SelectList(db.Models, "ModelId", "ModelName", console.ModelID);
            return View(console);
        }

        // GET: Consoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Console console = db.Consoles.Find(id);
            if (console == null)
            {
                return HttpNotFound();
            }
            return View(console);
        }

        // POST: Consoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Console console = db.Consoles.Find(id);
            db.Consoles.Remove(console);
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
