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
    public class GamesController : Controller
    {
        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();

        // GET: Games
        public ActionResult Index()
        {
            var games = db.Games.Include(g => g.GameSystem).Include(g => g.Items);
            
            return View(games.ToList());
        }

        // GET: Games/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }
        public PartialViewResult _Items(int id)
        {
            Game game = db.Games.Find(id);
            var items = game.Items;
            
            return PartialView("_Itmes", items);
        }

        [HttpPost]
        public ActionResult DetailItems(int id)
        {
            var item = db.Games.Include(g => g.Items).Where(g => g.GameId == id);
            //var items = db.Items.Where(i => itemIds.Contains(i.ItemId));
            return PartialView("_Items");
        }

        // GET: Games/Create
        public ActionResult Create()
        {
            ViewBag.System = new SelectList(db.GameSystems, "SystemId", "SystemName");
            
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GameId,Title,System,Publisher,DateOfRelease")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.System = new SelectList(db.GameSystems, "SystemId", "SystemName", game.System);
            return View(game);
        }
        //Me trying...
        public ActionResult AddItem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameId = id;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");

            return View();
        }

        [HttpPost]
        public ActionResult AddItem([Bind(Include = "ConditionId")] Item item, int GameId)
        {
            if (ModelState.IsValid)
            {

                item.TypeId = 1;
                db.Items.Add(item);
                db.SaveChanges();
                Game game = db.Games.Find(GameId);
                game.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GameId = GameId;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            return View(item);


        }
        // GET: Games/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            ViewBag.System = new SelectList(db.GameSystems, "SystemId", "SystemName", game.System);
            return View(game);
        }

        public ActionResult _FilterSystem()
        {
            return PartialView("_FilterSystem");
        }
        public ActionResult _Games()
        {
            var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            //var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            return PartialView("_Games", games);
        }
        [HttpPost]
        public ActionResult Systems()
        {
            var filters = db.GameSystems;
            return PartialView("_FilterSystem", filters);
        }
       [HttpPost]
        public ActionResult Filter(string filterText, int[] selected)
        {
            var selectedgames = db.Games.Include(g => g.GameSystem).Where(g => g.GameSystem.SystemId== null);
            var filteredgames = db.Games.Include(g => g.GameSystem).Where(g => g.Title.Contains(filterText));

            if (selected != null)
            {
                selectedgames = db.Games.Include(g => g.GameSystem).Where(g => selected.Contains(g.GameSystem.SystemId));
            }
            return PartialView("_Games", filteredgames.Except(selectedgames));
        }
        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GameId,Title,System,Publisher,DateOfRelease")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.System = new SelectList(db.GameSystems, "SystemId", "SystemName", game.System);
            return View(game);
        }

        // GET: Games/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = db.Games.Find(id);
            db.Games.Remove(game);
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
