using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GameStoreDB1.Models
{
    public partial class AddingItem: IEnumerable<AddingItem>
    {
        public IEnumerable<GameStoreDB1.Game> Game { get; set; }

        public IEnumerable<GameStoreDB1.Item> Item { get; set; }
        IEnumerable<GameStoreDB1.Console> Console { get; set; }
        IEnumerable<GameStoreDB1.Accessory> Accessory { get; set; }

        public int AccId { get; set; }
        public int AccType { get; set; }
        public string Discription { get; set; }

        public int GameId { get; set; }
        public string Title { get; set; }
        public int System { get; set; }
        public string Publisher { get; set; }
        public Nullable<System.DateTime> DateOfRelease { get; set; }

        public int ConsoleId { get; set; }
        public Nullable<int> ModelID { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<int> Color { get; set; }

        public int ItemId { get; set; }
        public Nullable<int> ConditionId { get; set; }
        public Nullable<int> TypeId { get; set; }

        public virtual GameSystem GameSystem { get; set; }
        
       public AddingItem()
        {
            this.Game = new HashSet<Game>();
            this.Item = new HashSet<Item>();
        }

        public IEnumerator<AddingItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}