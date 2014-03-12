using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services;
using System.Xml;
using Contingency;

namespace GameService
{
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class GameDataService : WebService
    {
        public string GameFile = "c:\\games.xml";

        public XmlDocument _doc = null;
        public XmlDocument Doc
        {
            get
            {
                if (_doc == null)
                {

                    _doc = new XmlDocument();
                    if (File.Exists(GameFile))
                    {
                        _doc.Load(GameFile);
                    }
                    else
                    {
                        _doc.LoadXml("<Games/>");
                    }
                }

                return _doc;
            }
        }

        public void ReloadDoc()
        {
            Doc.Load(GameFile);
        }

        public void SaveDoc(XmlDocument doc)
        {
            doc.Save(GameFile);
        }

        [WebMethod]
        public XmlNode GetState(string gameId, string playerId)
        {
            XmlNode node = Doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            if (node == null)
                return null;

            return node.SelectSingleNode("GameState");
        }

        [WebMethod]
        public XmlNode GetRefreshedData(string gameId, string playerId)
        {
            if (!RefreshAvailable(gameId, playerId))
            {
                return null;
            }

            return GetState(gameId, playerId);
        }

        [WebMethod]
        public string GetTeam(string gameId, string playerId)
        {

            XmlNode node = Doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

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
        public bool SendState(string gameId, XmlNode state, string playerId, string team)
        {
            if (!NeedsTurn(gameId, playerId, team))
            {
                return false;
            }

            XmlNode node = Doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            if (node == null)
            {
                node = Doc.CreateElement("Game");
                node.Attributes.Append(Doc.CreateAttribute("Id"));
                node.Attributes[0].Value = gameId;
                Doc.DocumentElement.AppendChild(node);
            }

            XmlNode player = node.SelectSingleNode("Players/Player[@ID='" + playerId + "']");

            bool host = false;
            bool newGame = false;
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
                    parentNode = Doc.CreateElement("Players");
                    node.AppendChild(parentNode);
                    host = true;
                    newGame = true;
                }

                XmlNode playerNode = Doc.CreateElement("Player");

                playerNode.Attributes.Append(Doc.CreateAttribute("ID"));
                playerNode.Attributes["ID"].Value = playerId;

                playerNode.Attributes.Append(Doc.CreateAttribute("Team"));
                playerNode.Attributes["Team"].Value = team;

                playerNode.Attributes.Append(Doc.CreateAttribute("Refresh"));
                playerNode.Attributes["Refresh"].Value = "false";

                parentNode.AppendChild(playerNode);
            }


            // consolidate states
            XmlNode currentStateNode = node.SelectSingleNode("GameState");

            if (host)
            {
                if (currentStateNode != null)
                    currentStateNode.ParentNode.RemoveChild(currentStateNode);

                node.AppendChild(Doc.ImportNode(state, true));

                if (newGame)
                {
                    XmlNode turnNode = Doc.CreateElement("Turns");

                    List<string> teams = new List<string>();
                    foreach (XmlNode unitNode in node.SelectNodes("GameState/UnitList/Unit"))
                    {
                        string teamValue = unitNode.Attributes["Team"].Value;

                        if (!teams.Contains(teamValue))
                        {
                            teams.Add(teamValue);
                            turnNode.AppendChild(Doc.CreateElement("Team"));
                            turnNode.ChildNodes[turnNode.ChildNodes.Count - 1].Attributes.Append(Doc.CreateAttribute("Name"));
                            turnNode.ChildNodes[turnNode.ChildNodes.Count - 1].Attributes[0].Value = teamValue;
                        }
                    }

                    node.AppendChild(turnNode);
                }
                else
                {
                    XmlNode x = node.SelectSingleNode("Turns/Team[@Name='" + team + "']");
                    x.ParentNode.RemoveChild(x);
                }
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
                    currentTeamUnitNode.AppendChild(Doc.ImportNode(orderNodeNew, true));
                }

                XmlNode x = node.SelectSingleNode("Turns/Team[@Name='" + team + "']");
                x.ParentNode.RemoveChild(x);
            }

            if (node.SelectNodes("Turns/Team").Count == 0)
            {
                XmlNode turnNode = node.SelectSingleNode("Turns");
                foreach (XmlNode playerNode in node.SelectNodes("Players/Player"))
                {
                    turnNode.AppendChild(Doc.CreateElement("Team"));
                    turnNode.ChildNodes[turnNode.ChildNodes.Count - 1].Attributes.Append(Doc.CreateAttribute("Name"));
                    turnNode.ChildNodes[turnNode.ChildNodes.Count - 1].Attributes[0].Value = playerNode.Attributes["Team"].Value;

                    playerNode.Attributes["Refresh"].Value = "true";
                }
            }

            SaveDoc(Doc);

            return true;
        }

        [WebMethod]
        public bool NeedsTurn(string gameId, string playerId, string team)
        {
            XmlNode node = Doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            if (node == null)
                return true;

            XmlNode x = node.SelectSingleNode("Turns/Team[@Name='" + team + "']");

            return x != null;
        }

        [WebMethod]
        public bool RefreshAvailable(string gameId, string playerId)
        {
            XmlNode node = Doc.SelectSingleNode("//Game[@Id='" + gameId + "']");

            if (node == null)
                return true;

            XmlNode x = node.SelectSingleNode("Players/Player[@ID='" + playerId + "']");
            if (x == null)
            {
                return false;
            }

            return x.Attributes["Refresh"].Value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

    }
}