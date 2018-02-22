using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameStoreDB1;
using GameStoreDB1.Controllers;
using System.Web.UI;

namespace GameStoreDB1.Models
{
    public class ItemsController : Controller
    {

        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();
       
        
        
        // GET: Items
        public ActionResult Index()
        {
            var items = db.Items.Include(i => i.Condition).Include(i => i.Type);
            return View(items.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        public ActionResult AddItem(int? id, string type)
        {
            
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            return View();
        }


        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "ItemType");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemId,ConditionId,TypeId")] Item item )
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(item);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1", item.ConditionId);
            
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "ItemId,ConditionId,TypeId")] Item item)
        {
            
           
            if (ModelState.IsValid)
            {
                /*switch ()
                {
                    case "Games":
                        item.TypeId = 1;
                        Game game = db.Games.Find();
                        game.Items.Add(item);
                        break;
                    case "Accessories":
                        item.TypeId = 2;
                        Accessory acc = db.Accessories.Find(i);
                        acc.Items.Add(item);
                        break;
                    case "Consoles":
                        item.TypeId = 3;
                        Console console = db.Consoles.Find();
                        console.Items.Add(item);
                        break;

                }*/
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            return View(item);
        }
        // GET: Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1", item.ConditionId);
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "ItemType", item.TypeId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemId,ConditionId,TypeId")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1", item.ConditionId);
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "ItemType", item.TypeId);
            return View(item);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);

            var games = db.Games.Include(g => g.Items).Where(g => g.Items.Equals(id));
            ///TO-DO
            db.Items.Remove(item);
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
