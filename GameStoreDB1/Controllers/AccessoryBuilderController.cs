using System;
using System.Collections;
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
    public class AccessoryBuilderController : Controller
    {

        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();
        

        // GET: AccessoryBuilder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            var mod = db.Models.Include(m => m.GameSystem).OrderBy(m => m.GameSystem.SystemName).ThenBy(m => m.ModelName);
            

            
            ViewBag.AccType = new SelectList(db.AccTypes, "AccTypeId", "Name");
            ViewBag.SystemModels = new MultiSelectList(mod, "ModelId", "ModelName", "GameSystem.SystemName");
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Include = "AccId, AccType, Discription")] Accessory accessory,List<int> SystemModels)
        {
            

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
}
    }
