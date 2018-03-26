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
                filteredgames = filteredgames.Where(g => selected.Contains(g.GameSystem.SystemId));
            }
            return PartialView("_Games", filteredgames);
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
            var models = db.Models.Include(m=>m.GameSystem).Except(db.Models.Where(m=> m.ModelName.Contains("All")));
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
            
            var consoles = db.Consoles.Include(c => c.Model.GameSystem).Include(c => c.Items);
            if(sysSelect !=null && modSelect != null && colSelect != null)
            {
                consoles = consoles.Where(c => sysSelect.Contains((int)c.Model.SystemId)&& modSelect.Contains((int)c.ModelID)&& colSelect.Contains((int)c.Color));
            }
            else if(sysSelect != null && modSelect != null)
            {
                consoles = consoles.Where(c => sysSelect.Contains((int)c.Model.SystemId) && modSelect.Contains((int)c.ModelID));
            }
            else if(modSelect != null && colSelect != null)
            {
                consoles = consoles.Where(c => modSelect.Contains((int)c.ModelID) && colSelect.Contains((int)c.Color));
            }
            else if(sysSelect != null && colSelect != null)
            {
                consoles = consoles.Where(c => sysSelect.Contains((int)c.Model.SystemId) && colSelect.Contains((int)c.Color));
            }
            else if(sysSelect != null)
            {
                consoles = consoles.Where(c => sysSelect.Contains((int)c.Model.SystemId));
            }
            else if (modSelect != null)
            {
                consoles = consoles.Where(c => modSelect.Contains((int)c.ModelID));
            }
            else if (colSelect != null)
            {
                consoles = consoles.Where(c => colSelect.Contains((int)c.Color));
            }
                return PartialView("_Consoles", consoles);
        }

        // Thanks to Mike Richards who commented on the answer here https://stackoverflow.com/questions/2539442/linq-array-property-contains-element-from-another-array. 
        //It was the key I needed to understand the array to hashset used both here and in ConsoleDisplay.
        [HttpPost]
        public ActionResult AccDisplay(int[] sysSelect, int[] modSelect)
        {
            var selectedAcc = db.Accessories.Where(a=>a.AccId == null);
            var acc = db.Accessories.Include(a => a.AccType1).Include(a => a.Items);
       
          if (sysSelect != null && modSelect != null)
            {
                acc = acc.Where(a => a.Models.Any(id => sysSelect.Contains(id.ModelId) || modSelect.Contains(id.ModelId)));
            }
    
            else if (sysSelect != null)
            {
                acc = acc.Where(a => a.Models.Any(id => sysSelect.Contains((int)id.SystemId)));
            }
            else if (modSelect != null)
            {
                acc = acc.Where(a => a.Models.Any(id => modSelect.Contains(id.ModelId)));
            }

            return PartialView("_Accessories", acc);
        }

        //Cart
        public ActionResult Cart()
        {
            return View();
        }
        public ActionResult _CartSymbol()
        {
            ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");
        }
        [HttpPost]
        public ActionResult AddAccToCart(int? id)
        {
            ViewBag.cart = 0;
            var acc = db.Accessories.Where(a=>a.AccId ==id).Include(a=>a.Items).ToArray() ;
            var items = acc[0].Items.ToArray();

            AddToCart(items);
            ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");

        }
        [HttpPost]
        public ActionResult AddGameToCart(int? id)
        {
            ViewBag.cart = 0;
            var acc = db.Games.Where(a => a.GameId == id).Include(a => a.Items).ToArray();
            var items = acc[0].Items.ToArray();

            AddToCart(items);
            ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");

        }
        [HttpPost]
        public ActionResult AddConToCart(int? id)
        {
            ViewBag.cart = 0;
            var acc = db.Consoles.Where(a => a.ConsoleId == id).Include(a => a.Items).ToArray();
            var items = acc[0].Items.ToArray();

            AddToCart(items);
            ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");

        }
        public void AddToCart(Item[] items)
        {
            if (Session["cart"] == null)
            {
                List<int> li = new List<int>();
                li.Add(items[0].ItemId);
                Session["cart"] = li;


                Session["count"] = 1;
            }
            else
            {
                List<int> li = (List<int>)Session["cart"];
                for (int i = 0; i <= items.Length - 1; i++)
                {
                    if (!li.Contains(items[i].ItemId))
                    {
                        li.Add(items[i].ItemId);
                        Session["cart"] = li;

                        Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                        break;
                    }
                }
            }
        }

    }
}