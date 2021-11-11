using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utility
{
    public static class RNG
    {
        private static Random rand;
        public static void initWithSeed(int seed)
        {
            rand = new Random(seed);
        }
        public static int getRandomInt(int minInclusive, int maxExclusive)
        {
            if (rand == null)
            {
                rand = new Random();
            }
            return rand.Next(minInclusive, maxExclusive);
        }
    }
}
