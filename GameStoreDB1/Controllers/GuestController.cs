using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameStoreDB1;
using System.Web.Routing;

namespace GameStoreDB1.Controllers
{
    public class GuestController : Controller
    {
        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();
        // GET: Guest
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Accessories()
        {
            return View();
        }
        public ActionResult Cart()
        {
            return View();
        }
        public ActionResult Consoles()
        {
            return View();
        }
        public ActionResult Games()
        {
            return View();
        }
        public ActionResult _FilterSystem()
        {
            return PartialView("_FilterSystem");
        }
        public ActionResult _Games()
        {
            var games = db.Games.Include(g => g.GameSystem).Include(g=>g.Items);
            //var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            return PartialView("_Games", games);
        }
        public ActionResult _Accessories()
        {
            var accessories = db.Accessories.Include(a => a.AccType1).Include(a => a.Items);
            return PartialView("_Accessories",accessories);
        }
        public ActionResult _Consoles()
        {
            var consoles = db.Consoles.Include(c => c.Model.GameSystem).Include(c => c.Items);
            return PartialView("_Consoles", consoles);
        }
        [HttpPost]
        public ActionResult Filter(string filterText, int[] selected)
        {
            var selectedgames = db.Games.Include(g => g.GameSystem).Where(g => g.GameSystem.SystemId == null);
            var filteredgames = db.Games.Include(g => g.GameSystem).Where(g => g.Title.Contains(filterText));

            if (selected != null)
            {
                selectedgames = db.Games.Include(g => g.GameSystem).Where(g => selected.Contains(g.GameSystem.SystemId));
            }
            return PartialView("_Games", filteredgames.Except(selectedgames));
        }
        [HttpPost]
        public ActionResult ShowColor()
        {
            var colors = db.Colors;
            return PartialView("_Colors", colors);
        }
    }
}