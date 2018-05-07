// check why it works in google but not explorer  + fixed ( cahnged == to .equals())
// formating bodyM


// figure out why can't add more to cart after item is removed  ++ fixed!
// get cart sybol to pdate after remove button is pushed  ++ works except from 1 to 0
// email contact information and list of items ++ works
// remove items from inventory ++ Works

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
using System.Net.Mail;
using System.Web.Helpers;
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
                Boolean newItem = true;
                List<int[]> li = (List<int[]>)Session[cart];
            
                    foreach (var ld in li)
                    {
                    newItem = false;
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
                        else
                        {
                        newItem = true;
                        }
                    }
                if (newItem)
                {
                    int[] idCount = { id, 1 };
                    li.Add(idCount);
                    Session[cart] = li;

                }
                else
                {
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
                        Session["count"] = Convert.ToInt32(Session["count"]) - 1;

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
        public ActionResult Checkout(string fName, string lName, string email)
        {
            var bodyM = "An online order has been placed for the following items: \r\n";
            List<int[]> cartGames = (List<int[]>)Session["CartG"];
            List<int[]> cartCon = (List<int[]>)Session["CartC"];
            List<int[]> cartAcc = (List<int[]>)Session["CartA"];
            if (cartGames != null)
            {
                bodyM += "- Games \n";
                foreach (var game in cartGames)
                {
                    int gameId = game[0];
                    var ga = db.Games.Where(g => g.GameId == gameId).Include(g => g.Items).Include(g => g.GameSystem).ToArray();
                    var items = ga[0].Items.ToArray();

                    for (var i = 1; i <= game[1]; i++)
                    {
                        try
                        {
                            bodyM += "--  "+ga[0].Title + ", " + ga[0].GameSystem.SystemName + " \n";
                            //remove item from database
                            var games = db.Games.Include(g => g.Items).SingleOrDefault(g => g.GameId == gameId);
                            if (games != null)
                            {
                                var item = games.Items.ToList();
                                games.Items.Remove(item[0]);
                              
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
            if (cartCon != null)
            {
                bodyM += "- Consoles \n";
                foreach (var con in cartCon)
                {
                    int conId = con[0];
                    var cons = db.Consoles.Where(c => c.ConsoleId == conId).Include(c => c.Items).Include(c => c.Model).ToArray();
                    var items = cons[0].Items.ToArray();

                    for (var i = 1; i <= con[1]; i++)
                    {
                        try
                        {
                            bodyM += "--  " + cons[0].Model.GameSystem.SystemName + ", " + cons[0].Model.ModelName + ", " + cons[0].SerialNumber + " \n";
                            //remove item from database
                            var console = db.Consoles.Include(g => g.Items).SingleOrDefault(g => g.ConsoleId == conId);
                            if (console != null)
                            {
                                var item = console.Items.ToList();
                                console.Items.Remove(item[0]);

                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
            if (cartAcc != null)
            {
                bodyM += "- Accessories \n";
                foreach (var acc in cartAcc)
                {
                    int accId = acc[0];
                    var acces = db.Accessories.Where(a => a.AccId == accId).Include(a => a.Items).Include(a => a.Models).Include(a=>a.Games).ToArray();
                    var items = acces[0].Items.ToArray();
                    var model = acces[0].Models.ToArray();
                    var games = acces[0].Games.ToArray();

                    for (var i = 1; i <= acc[1]; i++)
                    {
                        try
                        {
                            bodyM += "--  " + acces[0].Discription;
                            foreach(var mod in model)
                            {
                                bodyM += ", For the " + mod.GameSystem.SystemName + ": " + mod.ModelName;
                            }
                            foreach(var game in games)
                            {
                                bodyM += ", with " + game.Title;
                            }
                            bodyM += " \n";
                            //remove item from database
                            var accessory = db.Accessories.Include(g => g.Items).SingleOrDefault(g => g.AccId == accId);
                            if (accessory != null)
                            {
                                var item = accessory.Items.ToList();
                                accessory.Items.Remove(item[0]);

                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
            bodyM += "Contact information: " + fName + " " + lName + "\n " + email;
            MailMessage message = new MailMessage();
            message.To.Add("Amy.balser@hotmail.com");  
            message.From = new MailAddress("webiiifinal@outlook.com"); 
            message.Subject = "Online request";
            message.Body = string.Format(bodyM);
            message.IsBodyHtml = true;
            //string feedback;
            SmtpClient smtp = new SmtpClient();
            

                try
                {
                 NetworkCredential credential = new NetworkCredential("webiiifinal@outlook.com", "Webpassword1");

                 smtp.Credentials = credential;
                 smtp.Host = "smtp-mail.outlook.com";
                 smtp.Port = 587;
                 smtp.EnableSsl = true;
                 smtp.Send(message);
                
                ViewBag.feedback = "Sucessful! \n The following has been sent to For Your Gaming Needs \n" + bodyM ;
                Session["CartG"] = null;
                Session["CartC"] = null;
                Session["CartA"] = null;
                Session["Count"] = 0;
            }
                catch (Exception ex)
                {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                ViewBag.feedback = "failed";
                }
                return PartialView ("_Feedback");
            
        }

        public ActionResult _Feedback()
        {
            return PartialView("_Feedback");
        }
        public ActionResult _checkout()
        {

            return PartialView("_Checkout");
        }
    }
}