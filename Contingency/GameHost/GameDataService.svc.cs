using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Contingency;

namespace GameHost
{
    public class GameDataService : IGameDataService
    {
        public GameState GetGameState(string id)
        {
            return null;
        }

        public void SendGameState(string id)
        {
            
        }
    }
}
