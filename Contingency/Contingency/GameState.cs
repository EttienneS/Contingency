using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Contingency.Units;

namespace Contingency
{
    [Serializable]
    public class GameState : ISerializable
    {
        public static List<Block> Blocks;
        public static List<Explosion> Explosions;
        public static List<Unit> Units;
        public static List<Projectile> Projectiles;

        //    public static GameState Current
        //    {
        //        return
        //    private new GameState(Blocks , Explosions , Units , Projectiles );
        //}

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Blocks", Blocks);
            info.AddValue("Explosions", Explosions);
            info.AddValue("Units", Units);
            info.AddValue("Projectiles", Projectiles);
        }

        protected GameState(SerializationInfo information, StreamingContext context)
        {
            Blocks = (List<Block>)information.GetValue("Blocks", typeof(List<Block>));
            Explosions = (List<Explosion>)information.GetValue("Explosions", typeof(List<Explosion>));
            Units = (List<Unit>)information.GetValue("Units", typeof(List<Unit>));
            Projectiles = (List<Projectile>)information.GetValue("Projectiles", typeof(List<Projectile>));
        }

    }
}