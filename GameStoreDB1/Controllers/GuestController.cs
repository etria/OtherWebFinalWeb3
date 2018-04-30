﻿// figure out why can't add more to cart after item is removed
// get cart sybol to pdate after remove button is pushed
// email contact information and list of items
// remove items from inventory
// check why works it works in google but not explorer
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
//...
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

            AddToCart((int)id,"cartA", items.Count());
            //ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");

        }
        [HttpPost]
        public ActionResult AddGameToCart(int? id)
        {
            ViewBag.cart = 0;
            var game = db.Games.Where(a => a.GameId == id).Include(a => a.Items).ToArray();
            var items = game[0].Items.ToArray();

            AddToCart((int)id, "cartG", items.Count());
            //ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");

        }
        [HttpPost]
        public ActionResult AddConToCart(int? id)
        {
            ViewBag.cart = 0;
            var acc = db.Consoles.Where(a => a.ConsoleId == id).Include(a => a.Items).ToArray();
            var items = acc[0].Items.ToArray();

            AddToCart((int)id,"cartC", items.Count());
            //ViewBag.cart = Session["count"];
            return PartialView("_CartSymbol");

        }
        public void AddToCart(int id, string cart, int items)
        {
            if (Session[cart] == null)
            {
                List<int[]> li = new List<int[]>();
                int[] idCount = { id, 1 };
                li.Add(idCount);
                Session[cart] = li;

                if (Session["count"] == null){
                    Session["count"] = 1;
                }
                else{
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
            }
            else
            {
                List<int[]> li = (List<int[]>)Session[cart];
                if (!li[0].Contains(id))
                {
                        int[] idCount = { id, 1 };
                        li.Add(idCount);
                        Session[cart] = li;

                }
                else {
                    foreach (var ld in li)
                    {
                        if (ld[0] == id)
                        {
                            if (ld[1] < items)
                            {
                                ld[1] += 1;
                            }
                            else
                            {
                                goto NOTADDED;
                                //error
                            }
                        }
                    }
                }

                Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                NOTADDED:;

            }
        }

        public ActionResult _CartGames()
        {
            var game = db.Games.Where(g => g.GameId == null);
            List<int[]> li = (List<int[]>)Session["cartG"];
            if (li !=null)
            {
                List<int> list = new List<int>();
                foreach (var liId in li)
                {
                    list.Add(liId[0]);
                }
                game = db.Games.Where(g => list.Contains(g.GameId));
            }
          
            return PartialView("_CartGames",game);
        }
        public ActionResult _CartConsoles()
        {
            var con = db.Consoles.Where(c => c.ConsoleId==null);
            List<int[]> li = (List<int[]>)Session["cartC"];
            if (li!=null)
            {
                List<int> list = new List<int>();
                foreach (var liId in li)
                {
                    list.Add(liId[0]);
                }
                 con = db.Consoles.Where(c => list.Contains(c.ConsoleId));
            }
            return PartialView("_CartConsoles", con);
        }
        public ActionResult _CartAcc()
        {
            var acc = db.Accessories.Where(a => a.AccId==null);
            List<int[]> li = (List<int[]>)Session["cartA"];
            if (li!=null)
            {
                List<int> list = new List<int>();
                foreach (var liId in li)
                {
                    list.Add(liId[0]);
                }
               acc = db.Accessories.Where(a => list.Contains(a.AccId));
            }
            return PartialView("_CartAcc", acc);
        }
        [HttpPost]
        public ActionResult RemoveGameFromCart(int id)
        {
            List<int[]> cartGames = (List<int[]>)Session["CartG"];
            foreach (var ld in cartGames)
            {
                if (ld[0] == id)
                {
                    if (ld[1] >= 2)
                    {
                        ld[1] -= 1;
                        Session["count"] = Convert.ToInt32(Session["count"]) - 1;

                    }
                    else
                    {
                        cartGames.Remove(ld);
                        Session["count"] = Convert.ToInt32(Session["count"]) - 1;

                        break;
                    }
                }
            }
            List<int> list = new List<int>();
            foreach (var liId in cartGames)
            {
                list.Add(liId[0]);
            }
            var games = db.Games.Where(g => list.Contains(g.GameId));
            Session["CartG"] = cartGames;
            return PartialView("_CartGames", games);
        }
        [HttpPost]
        public ActionResult RemoveConFromCart(int id)
        {
            List<int[]> cartCon = (List<int[]>)Session["CartC"];
            
            foreach (var ld in cartCon)
            {
                if (ld[0] == id)
                {
                    if (ld[1] >= 2)
                    {
                        ld[1] -= 1;
                        Session["count"] = Convert.ToInt32(Session["count"])- 1;

                    }
                    else
                    {
                        cartCon.Remove(ld);
                        Session["count"] = Convert.ToInt32(Session["count"]) - 1;

                        break;
                    }
                }
            }
            List<int> list = new List<int>();
            foreach (var liId in cartCon)
            {
                list.Add(liId[0]);
            }
            var cons = db.Consoles.Where(c => list.Contains(c.ConsoleId));
            Session["Cart"] = cartCon;

            return PartialView("_CartConsoles", cons);
        }
        [HttpPost]
        public ActionResult RemoveAccFromCart(int id)
        {
            List<int[]> cartAcc = (List<int[]>)Session["CartA"];
            foreach (var ld in cartAcc)
            {
                if (ld[0] == id)
                {
                    if (ld[1] >= 2)
                    {
                        ld[1] -= 1;
                        Session["count"] = Convert.ToInt32(Session["count"]) - 1;

                    }
                    else
                    {
                        cartAcc.Remove(ld);
                        Session["count"] = Convert.ToInt32(Session["count"]) - 1;

                        break;
                    }
                }
            }
            List<int> list = new List<int>();
            foreach (var liId in cartAcc)
            {
                list.Add(liId[0]);
            }
            var acc = db.Accessories.Where(a => list.Contains(a.AccId));
            Session["Cart"] = cartAcc;

            return PartialView("_CartAcc", acc);
        }
        [HttpPost]
        public ActionResult Checkout()
        {
            List<int> cartGames = (List<int>)Session["CartG"];
            List<int> cartCon = (List<int>)Session["CartC"];
            List<int> cartAcc = (List<int>)Session["CartA"];


            return PartialView("_Checkout");
        }
        public ActionResult _checkout()
        {

            return PartialView("_Checkout");
        }
    }
}