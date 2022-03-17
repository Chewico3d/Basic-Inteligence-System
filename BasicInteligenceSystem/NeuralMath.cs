using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicInteligenceSystem
{
    public static class NeuralMath
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        public static float Sigmoid (float Value)
        {
            float DownValue = 1 + (float)Math.Pow(2.71f, -Value);
            return 1 / DownValue;

        }
        public static float DSigmoid(float SigmoidValue) => SigmoidValue * (1 - SigmoidValue);

        public static float LeakyRelu(float Value) => (Value > 0) ? Value : 0.01f * Value;
        public static float DLeakyRelu(float Value) => (Value > 0) ? 1 : 0.01f;

    }
}
