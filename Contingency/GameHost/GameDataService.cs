using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Contingency;
using Contingency.Units;

namespace GameHost
{
    [ServiceContract]
    public interface IGameDataService
    {
        [OperationContract]
        Special GetGameState(string id);

        [OperationContract]
        void SendGameState(string id);
    }
}
