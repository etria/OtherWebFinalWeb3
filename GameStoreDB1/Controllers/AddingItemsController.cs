// This is the Controller I built myself after playing around with the auto generated controllers and views.  
//The associated views were created and coded by me, based on (often copied and pasted) from the auto generated views.
//
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
    public class AddingItemsController : Controller
    {

        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();
        // GET: AddingItems
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Choose()
        {
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "ItemType");
            return View();
        }
        public ActionResult Game(int? id, List<int?> ids)
        {
            var games = db.Games.Include(g => g.GameSystem);
            if(ids == null){
                ids = new List<int?>();
            }
            if (id != null)
            {
                ids.Add(id);
                ViewBag.Ids = ids;
               
                
                //games=games.Where(g => g.GameId.Equals());
            }
            return View(games.ToList());
        }
        public ActionResult Accessory()
        {

            return View();
        }
        public ActionResult Console()
        {
            return View();
        }

        public ActionResult GameAddItem(int? id)
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
        public ActionResult ConsoleAddItem(int? id)
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
            ViewBag.ConsoleId = id;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");

            return View();
        }

        [HttpPost]
        public ActionResult Choose([Bind(Include = "TypeId")] Models.AddingItem addingItem)
        {
            switch (addingItem.TypeId)
            {
                case 1:
                    //must redirect to action to make sure that game model is created.
                    return RedirectToAction("Game");

                case 2:
                    return View("Accessory");

                case 3:
                    return View("Console");

            }
            return View();
        }
        [HttpPost]
        public ActionResult GameAddItem([Bind(Include = "GameId, ConditionId")] Models.AddingItem addingItem)
        {
            if (ModelState.IsValid)
            {

                Item item = new Item();
                item.ConditionId = addingItem.ConditionId;
                item.TypeId = 1;
                db.Items.Add(item);
                db.SaveChanges();
                Game game = db.Games.Find(addingItem.GameId);
                game.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Choose");
            }
            ViewBag.GameId = addingItem.GameId;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");
            return View(addingItem);


        }
        [HttpPost]
        public ActionResult ConsoleAddItem([Bind(Include ="ConsoleID, ConditionId")] Models.AddingItem addingItem)
        {
            if (ModelState.IsValid)
            {

                Item item = new Item();
                item.ConditionId = addingItem.ConditionId;
                item.TypeId = 3;
                db.Items.Add(item);
                db.SaveChanges();
                Console console = db.Consoles.Find(addingItem.ConsoleId);
                console.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Choose");
            }
            ViewBag.ConsoleId = addingItem.ConsoleId;

            ViewBag.ConditionId = new SelectList(db.Conditions, "ConditonId", "Condition1");

            return View();
        }
    
    }
}