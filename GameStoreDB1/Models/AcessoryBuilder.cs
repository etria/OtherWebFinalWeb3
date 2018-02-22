using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStoreDB1.Models
{
    public class AccessoryBuilder
    {
        public List<MultiSelectList> GameList  { get; set; }
        public string FilterText { get; set; }
        public string SelectedValue { get; set; }

        public AccessoryBuilder()
        {

        }

    }
}