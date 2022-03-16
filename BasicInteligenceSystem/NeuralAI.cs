using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicInteligenceSystem
{
    public class NeuralAI
    {

        //NeuralInformation
        public int[] NeuronLenght { get; private set; }
        public int LayersCount => NeuronLenght.Length;

        //Feed Foward Information4
        private float[][] Bias;
        private float[][] NeuronsValues;
        private float[][][] Weights;

        //Train Information
        private bool Traiable;
        private float[][] PartialNeuronDerivate;

        //User Input
        public float[] Inputs => NeuronsValues[0];
        public float[] Outpus => NeuronsValues[LayersCount - 1];

        //Funtion
        public NeuralAI(int[] NeuronCount)
        {
            //Copy the array to avoid editing it
            NeuronCount.CopyTo(NeuronLenght, 0);

            Weights = new float[LayersCount - 1][][];
            NeuronsValues = new float[LayersCount][];
            Bias = new float[LayersCount - 1][];
            //For every layer
            for(int x = 0; x < LayersCount; x++)
            {
                NeuronsValues[x] = new float[NeuronLenght[x]];

                //For every layer except the first
                if (x == 0)
                    continue;

                //The first layer doesn't have bias and weights
                Bias[x - 1] = new float[NeuronLenght[x]];
                Weights[x - 1] = new float[NeuronLenght[x]][];
                for (int y = 0; y < NeuronLenght[x]; y++)
                {
                    Weights[x - 1][y] = new float[NeuronLenght[x - 1]];

                }

            }

        }
        public void InitializeTrainingValues()
        {
            if (Traiable)
                return;
            //Maybye we only want the neural network for feed and not training
            Traiable = true;
            //the partial derivative for every neuron
            PartialNeuronDerivate = new float[LayersCount - 1][];
            for (int x = 0; x < LayersCount - 1; x++)
            {
                PartialNeuronDerivate[x] = new float[NeuronLenght[x + 1]];

            }

        }

        //FrontPorpagation
        public void FrontPropagate(float[] InputValues)
        {
            for (int x = 0; x < InputValues.Length; x++)
                Inputs[x] = InputValues[x];
                
            FrontPropagate();

        }
        public void FrontPropagate()
        {

            //For every Layer
            for(int x = 0; x < LayersCount - 1; x++)
            {
                //for every Neuron
                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    NeuronsValues[x + 1][y] = Bias[x][y];

                    //For every layer
                    for(int z = 0; z < NeuronLenght[x]; z++)
                    {

                    }

                }

            }

        }

        //Aditional Return values
        public float[][] GetValuesArray() => NeuronsValues;
        public float[][] GetBiasArray() => Bias;
        public float[][][] GetWeightArray() => Weights;

    }
}
