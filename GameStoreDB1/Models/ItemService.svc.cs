using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GameStoreDB1.Models
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ItemService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ItemService.svc or ItemService.svc.cs at the Solution Explorer and start debugging.
    public class ItemService : IItemService
    {
        private GameStoreLegacy_dbEntities db = new GameStoreLegacy_dbEntities();
        String type;
        int? otherId;
        int? myId;
        String itemTable = "Item";
        public int? updateInfo()
        {
            return otherId;
        }
        public void pullInfo(int? id, string type)
        {
            this.type=type;
            otherId = id;
        }
        public string updateType()
        {
            return type;
        }
      
    }
}
