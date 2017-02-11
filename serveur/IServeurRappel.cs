using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net.Security;

namespace serveur
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    
    public interface IServeurRappel
    {
        [OperationContract(IsOneWay = true, ProtectionLevel = ProtectionLevel.None)]
        void pseudoUsed();


        // TODO: Add your service operations here
    }


  
    
}
