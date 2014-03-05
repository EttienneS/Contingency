using System.Xml;
using Contingency.Units;

namespace Contingency
{
    public class GameState
    {
        public BlockList Blocks;

        public EffectList Effects;

        public ProjectileList Projectiles;

        public UnitList Units;

        public GameState()
        {
            Blocks = new BlockList();
            Effects = new EffectList();
            Units = new UnitList();
            Projectiles = new ProjectileList();
        }

        public static GameState Deserialize(XmlNode xml)
        {
            GameState state = new GameState();

            state.Blocks = BlockList.Deserialize(xml.SelectSingleNode("BlockList"));
            state.Effects = EffectList.Deserialize(xml.SelectSingleNode("EffectList"));
            state.Projectiles = ProjectileList.Deserialize(xml.SelectSingleNode("ProjectileList"));
            state.Units = UnitList.Deserialize(xml.SelectSingleNode("UnitList"));

            return state;
        }

        public XmlNode Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<GameState/>");

            doc.DocumentElement.AppendChild(doc.ImportNode(Units.Serialize(), true));
            doc.DocumentElement.AppendChild(doc.ImportNode(Effects.Serialize(), true));
            doc.DocumentElement.AppendChild(doc.ImportNode(Projectiles.Serialize(), true));
            doc.DocumentElement.AppendChild(doc.ImportNode(Blocks.Serialize(), true));

            return doc.DocumentElement;
        }
    }
}