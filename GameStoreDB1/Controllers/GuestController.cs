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
            var games = db.Games.Include(g => g.GameSystem).Include(g => g.Items);
            //var games = db.Games.Include(g => g.GameSystem).Where(g => g.GameId == null);
            return PartialView("_Games", games);
        }
        public ActionResult _Accessories()
        {
            var accessories = db.Accessories.Include(a => a.AccType1).Include(a => a.Items);
            return PartialView("_Accessories", accessories);
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
        public ActionResult ShowColor(int[] selected)
        {
            ViewData["colorArray"] = selected;
            var colors = db.Colors;
            return PartialView("_Colors", colors);
        }
        [HttpPost]
        public ActionResult ShowModel(int[] selected, int[] sysSelect)
        {
            ViewData["systemArray"] = sysSelect;
            ViewData["modelArray"] = selected;
            var models = db.Models.Include(m=>m.GameSystem);
            return PartialView("_Models", models);
        }
        [HttpPost]
        public ActionResult Systems(int[] selected)
        {
            ViewData["systemArray"] = selected;
            var filters = db.GameSystems;
            return PartialView("_SystemCheck", filters);
        }
        //dissplay as filters are seelcted/unselected on Console page
        [HttpPost]
        public ActionResult ConsoleDisplay(int[] sysSelect, int[] modSelect, int[] colSelect)
        {
            var selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => c.ConsoleId == null);
            var consoles = db.Consoles.Include(c => c.Model.GameSystem).Include(c => c.Items);
            if(sysSelect !=null && modSelect != null && colSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => sysSelect.Contains((int)c.Model.SystemId)|| modSelect.Contains((int)c.ModelID)|| colSelect.Contains((int)c.Color));
            }
            else if(sysSelect != null && modSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => sysSelect.Contains((int)c.Model.SystemId) || modSelect.Contains((int)c.ModelID));
            }
            else if(modSelect != null && colSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => modSelect.Contains((int)c.ModelID) || colSelect.Contains((int)c.Color));
            }
            else if(sysSelect != null && colSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => sysSelect.Contains((int)c.Model.SystemId) || colSelect.Contains((int)c.Color));
            }
            else if(sysSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => sysSelect.Contains((int)c.Model.SystemId));
            }
            else if (modSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => modSelect.Contains((int)c.ModelID));
            }
            else if (colSelect != null)
            {
                selectedConsoles = db.Consoles.Include(c => c.Model.GameSystem).Where(c => colSelect.Contains((int)c.Color));
            }
                return PartialView("_Consoles", consoles.Except(selectedConsoles));
        }

        // Thanks to Mike Richards who commented on the answer here https://stackoverflow.com/questions/2539442/linq-array-property-contains-element-from-another-array. 
        //It was the key I needed to understand the array to hashset.
        [HttpPost]
        public ActionResult AccDisplay(int[] sysSelect, int[] modSelect)
        {
            var selectedAcc = db.Accessories.Where(a=>a.AccId == null);
            var acc = db.Accessories.Include(a => a.AccType1).Include(a => a.Items);
       
          if (sysSelect != null && modSelect != null)
            {
                selectedAcc = db.Accessories.Where(a => a.Models.Any(id => sysSelect.Contains(id.ModelId) || modSelect.Contains(id.ModelId)));
            }
    
            else if (sysSelect != null)
            {
                selectedAcc = db.Accessories.Where(a => a.Models.Any(id => sysSelect.Contains((int)id.SystemId)));
            }
            else if (modSelect != null)
            {
                selectedAcc = db.Accessories.Where(a => a.Models.Any(id => modSelect.Contains(id.ModelId)));
            }

            return PartialView("_Accessories", acc.Except(selectedAcc));
        }
       
    }
}