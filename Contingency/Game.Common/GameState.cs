using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Contingency.Units;

namespace Contingency
{
    [Serializable]
    [DataContract]
    public class GameState : ISerializable
    {
        [DataMember]
        public List<Block> Blocks;

        [DataMember]
        public List<Explosion> Explosions;

        [DataMember]
        public List<Projectile> Projectiles;

        [DataMember]
        public List<Unit> Units;

        public GameState(List<Block> blocks, List<Explosion> explosions, List<Unit> units, List<Projectile> projectiles)
        {
            Blocks = blocks;
            Explosions = explosions;
            Units = units;
            Projectiles = projectiles;
        }

        public GameState()
        {
            Blocks = new List<Block>();
            Explosions = new List<Explosion>();
            Units = new List<Unit>();
            Projectiles = new List<Projectile>();
        }

        protected GameState(SerializationInfo information, StreamingContext context)
        {
            Blocks = (List<Block>)information.GetValue("Blocks", typeof(List<Block>));
            Explosions = (List<Explosion>)information.GetValue("Explosions", typeof(List<Explosion>));
            Units = (List<Unit>)information.GetValue("Units", typeof(List<Unit>));
            Projectiles = (List<Projectile>)information.GetValue("Projectiles", typeof(List<Projectile>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Blocks", Blocks);
            info.AddValue("Explosions", Explosions);
            info.AddValue("Units", Units);
            info.AddValue("Projectiles", Projectiles);
        }

        public static GameState ConsolidateGamesState(GameState state1, GameState state2, string fromTeam)
        {
            for (int i = 0; i < state1.Units.Count; i++)
            {
                if (state1.Units[i].Team != fromTeam)
                {
                    state1.Units.RemoveAt(i);
                }
            }

            for (int i = 0; i < state2.Units.Count; i++)
            {
                if (state2.Units[i].Team != fromTeam)
                {
                    state1.Units.Add(state2.Units[i]);
                }
            }


            return state1;
        }
    }
}