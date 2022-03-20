using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicInteligenceSystem
{
    /// <summary>
    /// A static class to manage the serialitzation and save of the neural network
    /// </summary>
    public static class NeuralSave
    {
        #region Rules
        /*
        General:
        -The first 4 bytes is an int that tells the number of layers the neural network have
        -There will enter a loop that will tell how many neurons have every layer
        -There will be a byte that contains the number 255 that will tell the loop has ended and this will tell that for the moment the read is correct

        -Look the code to understand more

        */
        #endregion

        //Vars
        private static int Layers;
        private static int[] NeuronLenght;
        private static float[][] Biases;
        private static float[][][] Weights;
        private static byte[] NeuralByteArray;
        private static int Index;

        public enum Errors
        {
            CouldNotReadInitialData,
            CouldNotReadWeightsAndBias,
            SomethingFaildInTheEnd,
            None
        };
        public static Errors ErrorGave;

        /// <summary>
        /// A simple Funtion to convert the entire neural network to a byte array
        /// </summary>
        /// <returns></returns>
        public static byte[] ConvertNeuralToByteArray(NeuralAI Network)
        {
            Layers = Network.LayersCount;
            NeuronLenght = Network.NeuronLenght;
            Biases = Network.GetBiasArray();
            Weights = Network.GetWeightArray();

            Index = 0;

            int ByteCountGeneral = GetByteCountInGeneral();
            int ByteCountBias = GetByteCountInBias();
            int ByteCountWeights = GetByteCountInWeights();

            int TotalAmountOfBytes = ByteCountGeneral + ByteCountBias * 2 + ByteCountWeights + 2;

            NeuralByteArray = new byte[TotalAmountOfBytes];

            //General
            WriteInt(Layers);
            for(int x = 0; x < Layers; x++)
                WriteInt(NeuronLenght[x]);
            
            Writebyte(170);//

            //Biases and weights
            for(int x = 0; x < Layers - 1; x++)
            {
                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    WriteFloat(Biases[x][y]);
                    for (int z = 0; z < NeuronLenght[x]; y++)
                        WriteFloat(Weights[x][y][z]);
                    Writebyte(14);
                }
            }
            Writebyte(255);

            return NeuralByteArray;
        }
        public static NeuralAI LoadNeuralNetwork(byte[] ByteArray)
        {
            Index = 0;

            Layers = ReadInt();
            NeuronLenght = new int[Layers];

            for(int x = 0; x < Layers; x++)
                NeuronLenght[x] = ReadInt();

            if (ReadByte() != 170)
            {
                ErrorGave = Errors.CouldNotReadInitialData;
                return null;

            }

            Biases = new float[Layers - 1][];
            Weights = new float[Layers - 1][][];

            for(int x = 0; x < Layers - 1; x++)
            {
                Biases[x] = new float[NeuronLenght[x + 1]];
                Weights[x] = new float[NeuronLenght[x + 1]][];

                for(int y = 0; y < NeuronLenght[x + 1]; y++)
                {
                    Weights[x][y] = new float[NeuronLenght[x]];
                    for (int z = 0; z < NeuronLenght[x]; z++)
                        Weights[x][y][z] = ReadFloat();

                }

                if (ReadByte() != 14)
                {
                    ErrorGave = Errors.CouldNotReadInitialData;
                    return null;

                }

            }
            if (ReadByte() != 255)
            {
                ErrorGave = Errors.SomethingFaildInTheEnd;
                return null;

            }
            else
                ErrorGave = Errors.None;

            return new NeuralAI(NeuronLenght, Biases,Weights);
            
        }
        /// <summary>
        /// A simple Funtion to convert the bias of the neural network to a byte array
        /// </summary>
        /// <returns></returns>

        private static void WriteInt(int Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            NeuralByteArray[Index] = Data[0];
            NeuralByteArray[Index + 1] = Data[1];
            NeuralByteArray[Index + 2] = Data[2];
            NeuralByteArray[Index + 3] = Data[3];

            Index += 4;

        }
        private static void WriteFloat(float Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            NeuralByteArray[Index] = Data[0];
            NeuralByteArray[Index + 1] = Data[1];
            NeuralByteArray[Index + 2] = Data[2];
            NeuralByteArray[Index + 3] = Data[3];

            Index += 4;

        }
        private static void Writebyte(byte Value)
        {
            NeuralByteArray[Index] = Value;

            Index++;

        }

        private static int ReadInt()
        {
            byte[] Data = new byte[]
            {
                NeuralByteArray[Index + 0],
                NeuralByteArray[Index + 1],
                NeuralByteArray[Index + 2],
                NeuralByteArray[Index + 3]
            };
            
            Index += 4;

            return BitConverter.ToInt32(Data, 0);

        }
        private static float ReadFloat()
        {
            byte[] Data = new byte[]
            {
                NeuralByteArray[Index + 0],
                NeuralByteArray[Index + 1],
                NeuralByteArray[Index + 2],
                NeuralByteArray[Index + 3]

            };

            Index += 4;
            
            return BitConverter.ToSingle(Data, 0);

        }
        private static byte ReadByte()
        {
            byte Value = NeuralByteArray[Index];
            Index++;

            return Value;

        }

        private static int GetByteCountInGeneral()
        {
            //The First byte
            int Count = 4;

            Count += Layers * 4;

            return Count;
        }
        private static int GetByteCountInBias()
        {
            //The First byte
            int Count = 0;
            for (int x = 0; x < Layers - 1; Layers++)
                Count += NeuronLenght[x + 1] * 4;

            return Count;
        }
        private static int GetByteCountInWeights()
        {
            //The First byte
            int Count = 0;
            for (int x = 0; x < Layers - 1; Layers++)
                for (int y = 0; y < NeuronLenght[x + 1]; y++)
                    Count += NeuronLenght[x] * 4;

            return Count;
        }

    }
}
