using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicInteligenceSystem
{
    /// <summary>
    /// The Main Class for the Neural Network
    /// </summary>
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
        private int TrainItirenations = 0;
        private float[][] PartialNeuronDerivate;
        //Total derivative is the cache to change the values until apply training
        private float[][][] WeightsTotalDerivative;
        private float[][] BiasTotalDerivative;
        //User Input
        public float TrainingRate = 0.01f;
        public float[] ExpectedOutput { get; private set; }

        //User Input
        public float[] Inputs => NeuronsValues[0];
        public float[] Outputs => NeuronsValues[LayersCount - 1];

        //Funtion
        public NeuralAI(int[] NeuronCount)
        {
            //Copy the array to avoid editing it
            NeuronLenght = NeuronCount;

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
        public NeuralAI(int[] NeuronCount, float[][] Bias, float[][][] Weights)
        {
            //Copy the array to avoid editing it
            NeuronLenght = NeuronCount;

            this.Weights = Weights;
            this.NeuronsValues = new float[LayersCount][];
            this.Bias = Bias;

            //For every layer
            for (int x = 0; x < LayersCount; x++)
                NeuronsValues[x] = new float[NeuronLenght[x]];

        }
        public void RanzomitzeValues()
        {
            for(int x = 0; x < LayersCount - 1; x++)
            {
                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    for (int z = 0; z < NeuronLenght[x]; z++)
                        Weights[x][y][z] = (float)NeuralMath.Random.NextDouble() * 2 - 1;

                    Bias[x][y] = (float)NeuralMath.Random.NextDouble() * 2 - 1;
                }
            }
        }
        public void InitializeTrainingValues()
        {
            //Maybye we only want the neural network for feed and not training
            Traiable = true;
            //the partial derivative for every neuron
            PartialNeuronDerivate = new float[LayersCount - 1][];
            ExpectedOutput = new float[NeuronLenght[LayersCount - 1]];

            WeightsTotalDerivative = new float[LayersCount - 1][][];
            BiasTotalDerivative = new float[LayersCount - 1][];

            for (int x = 0; x < LayersCount - 1; x++)
            {
                PartialNeuronDerivate[x] = new float[NeuronLenght[x + 1]];
                WeightsTotalDerivative[x] = new float[NeuronLenght[x + 1]][];

                BiasTotalDerivative[x] = new float[NeuronLenght[x + 1]];

                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    WeightsTotalDerivative[x][y] = new float[NeuronLenght[x]];

                }

            }

        }

        //FrontPorpagation
        public float[] FeedFoward(float[] InputValues)
        {
            for (int x = 0; x < InputValues.Length; x++)
                Inputs[x] = InputValues[x];
                
            FeedFoward();
            return Outputs;

        }
        public void FeedFoward()
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
                        //Multiply the last neurn value by the weight connected to it
                        NeuronsValues[x + 1][y] += Weights[x][y][z] * NeuronsValues[x][z];

                    }

                    //Last layer or hidden layer
                    if(x == LayersCount - 2)
                        NeuronsValues[x + 1][y] = NeuralMath.Sigmoid(NeuronsValues[x + 1][y]);
                    else
                        NeuronsValues[x + 1][y] = NeuralMath.LeakyRelu(NeuronsValues[x + 1][y]);

                }

            }

        }

        //Train with back propagation
        public void Train()
        {

            if (!Traiable) throw new ArgumentException("You must initialize training values to train");
            TrainItirenations++;

            //Reset Partial Derivatives
            for (int x = 0; x < LayersCount - 1; x++)
            {
                for (int y = 0; y < NeuronLenght[x + 1]; y++)
                    PartialNeuronDerivate[x][y] = 0;
            }

            //Calculate the last derivartives
            for (int y = 0; y < NeuronLenght[LayersCount - 1]; y++)
                PartialNeuronDerivate[LayersCount - 2][y] = (Outputs[y] - ExpectedOutput[y]) * 2;

            for (int x = LayersCount - 2; x >= 0; x--)
            {
                //First calculate the activation funtion derivative
                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    if(x == LayersCount - 2)//
                        PartialNeuronDerivate[x][y] *= NeuralMath.DSigmoid(NeuronsValues[x + 1][y]);
                    else
                        PartialNeuronDerivate[x][y] *= NeuralMath.DLeakyRelu(NeuronsValues[x + 1][y]);

                    BiasTotalDerivative[x][y] += PartialNeuronDerivate[x][y];
                    for(int z = 0; z < NeuronLenght[x]; z++)
                    {
                        WeightsTotalDerivative[x][y][z] += PartialNeuronDerivate[x][y] * NeuronsValues[x][z];
                        
                        if(x != 0)
                            PartialNeuronDerivate[x - 1][z] += PartialNeuronDerivate[x][y] * Weights[x][y][z];
                        
                    }

                }
            }

        }
        public void ApplyTraining()
        {
            if (TrainItirenations == 0)
                return;

            for(int x = 0; x < LayersCount - 1; x++)
            {
                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    for (int z = 0; z < NeuronLenght[x]; z++)
                        Weights[x][y][z] -= WeightsTotalDerivative[x][y][z] / (float)TrainItirenations * TrainingRate;
                    for (int z = 0; z < NeuronLenght[x]; z++)
                        WeightsTotalDerivative[x][y][z] = 0;

                    Bias[x][y] -= BiasTotalDerivative[x][y] / (float)TrainItirenations * TrainingRate;
                    BiasTotalDerivative[x][y] = 0;
                }

            }

            TrainItirenations = 0;

        }
        public float AverageError
        {
            get
            {
                float AverageError = 0;
                for (int x = 0; x < Outputs.Length; x++)
                {
                    float Error = Outputs[x] - ExpectedOutput[x];
                    AverageError += Error * Error;
                }
                return (float)AverageError / (float)Outputs.Length;

            }
        }

        //Aditional User Values
        public float[][] GetValuesArray() => NeuronsValues;
        public float[][] GetBiasArray() => Bias;
        public float[][][] GetWeightArray() => Weights;


    }
}
