using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameStoreDB1;
using System.Threading.Tasks;
using System.Web.Routing;

namespace GameStoreDB1.Controllers
{
    public class AccessoriesController : Controller
    {
        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();

        // GET: Accessories
        public ActionResult Index()
        {
            var accessories = db.Accessories.Include(a => a.AccType1).Include(a => a.Items);
            return View(accessories.ToList());
        }
        
        

        // GET: Accessories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accessory accessory = db.Accessories.Find(id);
            if (accessory == null)
            {
                return HttpNotFound();
            }
            return View(accessory);
        }


        [HttpPost]
        public ActionResult Filter(string filterText, int[] listed)
      
        
        {
            var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            var filteredgames = db.Games.Include(g => g.GameSystem).Where(g => g.Title.Contains(filterText));
            if (listed != null)
            {
                games = db.Games.Include(g => g.GameSystem).Where(g => listed.Contains(g.GameId));
            }


            return PartialView("_Games", filteredgames.Except(games));
        }
        [HttpPost]
        public ActionResult Selecting(string filterId, int[] listed)
          
        {

            var selectedgames = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);

            if (listed != null)
            {
                selectedgames=db.Games.Include(g => g.GameSystem).Where(g => listed.Contains(g.GameId));
            }
            
            ViewBag.Listed = String.Join(",", listed);

            return PartialView("_SelectedGames",selectedgames);
        }
        // GET: Accessories/Create
        public ActionResult Create()
        {
            var mod = db.Models.Include(m => m.GameSystem).OrderBy(m => m.GameSystem.SystemName).ThenBy(m => m.ModelName);
            
            ViewBag.AccType = new SelectList(db.AccTypes, "AccTypeId", "Name");
            ViewBag.SystemModels = new MultiSelectList(mod, "ModelId", "ModelName", "GameSystem.SystemName");
            return View();
            //return RedirectToAction("Create", "AcessoryBuilder");
        }
        public ActionResult _Games()
        {
            var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            //var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            return PartialView("_Games",games);
        }
        public ActionResult _SelectedGames()
        {
            var games = db.Games.Include(g => g.GameSystem).Where(g=>g.GameId ==null);
            return PartialView("_SelectedGames",games);
        }
        public ActionResult AddItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accessory accessory= db.Accessories.Find(id);
            if (accessory == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccId = id;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");

            return View();
        }

        [HttpPost]
        public ActionResult AddItem([Bind(Include = "ConditionId")] Item item, int AccId)
        {
            if (ModelState.IsValid)
            {

                item.TypeId = 2;
                db.Items.Add(item);
                db.SaveChanges();
                Accessory accessory = db.Accessories.Find(AccId);
                accessory.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccId = AccId;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            return View(item);
        }


           
        // POST: Accessories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccId, AccType, Discription")] Accessory accessory, List<int> SystemModels)
        {
            string list = Request.Form["Listed"];
            string[] listed = list.Split(',');
            foreach(string s in listed)
            {
                int t = Convert.ToInt32(s);
                accessory.Games.Add(db.Games.Find(t));
            }
            foreach (int s in SystemModels)
            {
                accessory.Models.Add(db.Models.Find(s));
            }
            if (ModelState.IsValid)
            {


                db.Accessories.Add(accessory);
                db.SaveChanges();

                return RedirectToAction("Create");
            }

            ViewBag.AccType = new SelectList(db.AccTypes, "AccTypeId", "Name", accessory.AccType);
            return View(accessory);
        }

        // GET: Accessories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accessory accessory = db.Accessories.Find(id);
            if (accessory == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccType = new SelectList(db.AccTypes, "AccTypeId", "Name", accessory.AccType);
            return View(accessory);
        }

        // POST: Accessories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccId,AccType,Discription")] Accessory accessory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accessory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccType = new SelectList(db.AccTypes, "AccTypeId", "Name", accessory.AccType);
            return View(accessory);
        }

        // GET: Accessories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accessory accessory = db.Accessories.Find(id);
            if (accessory == null)
            {
                return HttpNotFound();
            }
            return View(accessory);
        }

        // POST: Accessories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Accessory accessory = db.Accessories.Find(id);
            db.Accessories.Remove(accessory);
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

        public void Execute(RequestContext requestContext)
        {
            throw new NotImplementedException();
        }
    }
}
