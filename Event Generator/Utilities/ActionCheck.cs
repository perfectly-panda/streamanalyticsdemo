using System;
using System.Collections.Generic;
using System.Text;

namespace Event_Generator
{
    public static class ActionCheck
    {
        private static Random random = new Random();

        public static bool Check(float variability)
        {
            double percent = 1F;

            percent = 1F - variability;

            var variant = (random.NextDouble() - .5);

            percent = percent + variant;

            var testDouble = random.NextDouble();

            return percent >= testDouble;
        }

        public static int GenerateInt(int maxSize)
        {
            return random.Next(maxSize);
        }

        public static int GenerateInt(int maxSize, int variability, bool noNegativeValues = true)
        {

            var result = random.Next(maxSize) - (int)Math.Ceiling(random.Next(variability * 2) / 2.0);

            if (noNegativeValues && result < 0)
            {
                result = 0;
            }

            return result;
        }

        public static float GenerateFloat(float maxSize, float variability)
        {

            var next = random.NextDouble() * maxSize;

            var variable = (random.NextDouble() - .5) * variability;

            return (float)(next + variable);
            
        }
    }
}
