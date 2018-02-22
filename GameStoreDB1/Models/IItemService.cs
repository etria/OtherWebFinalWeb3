using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GameStoreDB1.Models
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IItemService" in both code and config file together.
    [ServiceContract]
    public interface IItemService
    {
        
        [OperationContract]
        int? updateInfo();

        [OperationContract]
        void pullInfo(int? id, string type);

        [OperationContract]
        string updateType();


        
    }
}
