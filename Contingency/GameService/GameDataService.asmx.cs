using System;
using System.Web.Services;
using System.Xml;
using Contingency;

namespace GameService
{
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class GameDataService : WebService
    {

        [WebMethod]
        public XmlNode GetState(string id)
        {
            return null;
        }

        [WebMethod]
        public void SendState(string id, XmlNode state)
        {
        }
    }
}