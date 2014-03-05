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

        public GameState(BlockList blocks, EffectList effects, UnitList units, ProjectileList projectiles)
        {
            Blocks = blocks;
            Effects = effects;
            Units = units;
            Projectiles = projectiles;
        }

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

            foreach (XmlNode node in xml.ChildNodes)
            {
                switch (node.Name)
                {
                    case "BlockList":
                        state.Blocks = BlockList.Deserialize(node);
                        break;

                    case "EffectList":
                        state.Effects = EffectList.Deserialize(node);
                        break;

                    case "ProjectileList":
                        state.Projectiles = ProjectileList.Deserialize(node);
                        break;

                    case "UnitList":
                        state.Units = UnitList.Deserialize(node);
                        break;
                }
            }

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