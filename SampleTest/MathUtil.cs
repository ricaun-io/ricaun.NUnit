using System;

namespace SampleTest
{
    public static class MathUtil
    {
        public static double Sum(double value, params double[] values)
        {
            double result = value;
            for (int i = 0; i < values.Length; i++)
            {
                result += values[i];
            }
            return result;
        }

        public static double Average(double value, params double[] values)
        {
            double result = Sum(value, values);
            return result / (1 + values.Length);
        }

        public static int Invert(int value)
        {
            return 1 / value;
        }
    }
}