using System;
using System.Collections.Generic;
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
        public XmlNode GetState(string gameId, string playerId)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"c:\\games.xml");

            XmlNode node = doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            return node.SelectSingleNode("GameState");
        }

        [WebMethod]
        public XmlNode GetTurnData(string gameId, string playerId)
        {
            return null;
        }

        [WebMethod]
        public string GetTeam(string gameId, string playerId)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"c:\\games.xml");

            XmlNode node = doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            if (node == null)
            {
                return "red"; // no players, default player 1
            }

            XmlNode player = node.SelectSingleNode("Players/Player[@ID='" + playerId + "']");
            if (player == null)
            {
                return "blue"; // some players but not current player, default player 2
            }

            return player.Attributes["Team"].Value;
        }

        [WebMethod]
        public void SendState(string gameId, XmlNode state, string playerId, string team)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"c:\\games.xml");

            XmlNode node = doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            if (node == null)
            {
                node = doc.CreateElement("Game");
                node.Attributes.Append(doc.CreateAttribute("Id"));
                node.Attributes[0].Value = gameId;
                doc.DocumentElement.AppendChild(node);
            }

            XmlNode player = node.SelectSingleNode("Players/Player[@ID='" + playerId + "']");

            bool host = false;
            if (player != null)
            {
                if (player.Attributes["Team"].Value == "red")
                {
                    host = true;
                }
            }
            else
            {
                XmlNode parentNode = node.SelectSingleNode("Players");

                if (parentNode == null)
                {
                    parentNode = doc.CreateElement("Players");
                    node.AppendChild(parentNode);
                }

                XmlNode playerNode = doc.CreateElement("Player");

                playerNode.Attributes.Append(doc.CreateAttribute("ID"));
                playerNode.Attributes["ID"].Value = playerId;
                playerNode.Attributes.Append(doc.CreateAttribute("Team"));
                playerNode.Attributes["Team"].Value = team;

                parentNode.AppendChild(playerNode);
            }

            // consolidate states
            XmlNode currentStateNode = node.SelectSingleNode("GameState");
            if (host)
            {
                if (currentStateNode != null)
                    currentStateNode.ParentNode.RemoveChild(currentStateNode);

                node.AppendChild(doc.ImportNode(state, true));
            }
            else
            {
                foreach (XmlNode currentTeamUnitNode in currentStateNode.SelectNodes("UnitList/Unit[@Team='" + team + "']"))
                {
                    string id = currentTeamUnitNode.Attributes["ID"].Value;

                    XmlNode stateUnitNode = state.SelectSingleNode("UnitList/Unit[@ID='" + id + "']");

                    XmlNode orderNode = currentTeamUnitNode.SelectSingleNode("OrderList");
                    orderNode.ParentNode.RemoveChild(orderNode);

                    XmlNode orderNodeNew = stateUnitNode.SelectSingleNode("OrderList");
                    currentTeamUnitNode.AppendChild(doc.ImportNode(orderNodeNew, true));
                }
            }

            doc.Save(@"c:\\games.xml");
        }
    }
}