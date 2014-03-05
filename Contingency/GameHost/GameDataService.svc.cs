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
    public class GameDataService : IGameDataService
    {
        public Special GetGameState(string id)
        {
            return new Special("sfasdf", 1f,2f);
        }

        public void SendGameState(string id)
        {
            
        }
    }
}
