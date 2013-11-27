using System;

namespace Contingency
{
    public static class Helper
    {
        public static Random Rand = new Random();

        public static float NextFloat()
        {
            var buffer = new byte[4];
            Rand.NextBytes(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }
    }
}