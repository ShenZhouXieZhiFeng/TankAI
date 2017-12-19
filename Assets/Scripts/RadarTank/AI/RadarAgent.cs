using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radar
{
    /// <summary>
    /// 智能类
    /// </summary>
    public class RadarAgent
    {
        //遗传算子
        public SGenome Genome;
        //神经网络
        CNeuronNet neuronNet;
        int neruonPerLyr = 4;
        int bias = 1;

        public double AgentScore {
            set {
                Genome.dFitness = value;
            }
            get {
                return Genome.dFitness;
            }
        }

        /// <summary>
        /// 权重数量
        /// </summary>
        public int WeightsCount
        {
            get
            {
                return neuronNet.GetNumOfWeight();
            }
        }

        public RadarAgent(int inputNum, int outputsNums, int hiddenLayers)
        {
            neuronNet = new CNeuronNet(inputNum, outputsNums, hiddenLayers, neruonPerLyr, bias);
            Genome = new SGenome(neuronNet.GetWeights());
        }

        /// <summary>
        /// 更新神经网络矩阵
        /// </summary>
        /// <param name="vecWeights"></param>
        public void RefreshNeuronNet()
        {
            neuronNet.PutWeights(Genome.vecWeights);
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] updateValue(double[] inputs)
        {
            return neuronNet.HandleInputs(inputs);
        }
    }

    #region 遗传算法相关
    public class SGenome
    {
        public List<double> vecWeights;
        public double dFitness;
        public SGenome(List<double> w) {
            dFitness = 0;
            vecWeights = w;
        }
        public double this[int index] {
            get {return vecWeights[index]; }
            set { vecWeights[index] = value; }
        }
    }
    #endregion

    #region 神经网络相关
    /// <summary>
    /// 神经网络类
    /// </summary>
    class CNeuronNet
    {
        int numInputs;//输入
        int numOutPuts;//输出
        int numHiddenLayers;//隐藏层数量
        int neuronsPerHiddenLyr;//每个层的神经细胞数目
        int bias = -1;

        List<SNeuronLayer> neuronLayers;

        public CNeuronNet(int inputNum, int outputsNums, int hiddenLayers, int neruonPerLyr, int _bias)
        {
            numInputs = inputNum;
            numOutPuts = outputsNums;
            numHiddenLayers = hiddenLayers;
            neuronsPerHiddenLyr = neruonPerLyr;
            bias = _bias;

            neuronLayers = new List<SNeuronLayer>();
            createNet();
        }

        /// <summary>
        /// 通过这个神经网络处理输入信号
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] HandleInputs(double[] inputs)
        {
            List<double> outs = new List<double>();
            int cWeight = 0;

            if (inputs.Length != numInputs)
                return outs.ToArray(); ;

            //使用神经网络矩阵处理
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                if (i > 0)
                {
                    //从第二层开始，输入为上一层的输出
                    inputs = outs.ToArray();
                }
                outs.Clear();
                cWeight = 0;
                for (int j = 0; j < neuronLayers[i].NeuronNums; ++j)
                {
                    double netinput = 0;

                    int numInputs = neuronLayers[i].Neurons[j].InputNums;

                    //处理输入
                    for (int k = 0; k < numInputs - 1; ++k)
                    {
                        netinput += neuronLayers[i].Neurons[j].NeuronWeight[k] * inputs[cWeight++];
                    }
                    //增加偏置
                    netinput += neuronLayers[i].Neurons[j].NeuronWeight[numInputs - 1] * bias;

                    outs.Add(netinput);

                    cWeight = 0;
                }
            }

            //使用激活函数处理
            for (int i = 0; i < outs.Count; i++)
            {
                outs[i] = MathTools.Sigmoid((float)outs[i]);
            }

            return outs.ToArray();
        }

        /// <summary>
        /// 获取权重集合
        /// </summary>
        /// <returns></returns>
        public List<double> GetWeights()
        {
            List<double> res = new List<double>();
            //层
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                //神经
                for (int j = 0; j < neuronLayers[i].NeuronNums; ++j)
                {
                    //权重
                    for (int k = 0; k < neuronLayers[i].Neurons[j].InputNums; ++k)
                    {
                        res.Add(neuronLayers[i].Neurons[j].NeuronWeight[k]);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 使用新的权重集合设置这个神经网络
        /// </summary>
        public void PutWeights(List<double> weights)
        {
            int index = 0;
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                //神经
                for (int j = 0; j < neuronLayers[i].NeuronNums; ++j)
                {
                    //权重
                    for (int k = 0; k < neuronLayers[i].Neurons[j].InputNums; ++k)
                    {
                        neuronLayers[i].Neurons[j].NeuronWeight[k] = weights[index++];
                    }
                }
            }
        }

        /// <summary>
        /// 获取权重数量
        /// </summary>
        /// <returns></returns>
        public int GetNumOfWeight()
        {
            int count = 0;
            for (int i = 0; i < numHiddenLayers + 1; ++i)
            {
                //神经
                for (int j = 0; j < neuronLayers[i].NeuronNums; ++j)
                {
                    //权重
                    for (int k = 0; k < neuronLayers[i].Neurons[j].InputNums; ++k)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        void createNet()
        {
            if (numHiddenLayers > 0)
            {
                //第一层
                neuronLayers.Add(new SNeuronLayer(neuronsPerHiddenLyr, numInputs));
                for (int i = 0; i < numHiddenLayers - 1; ++i)
                {
                    //中间层的神经细胞数都一样（也可以设置成不一样，但前后层要能够对应）
                    neuronLayers.Add(new SNeuronLayer(neuronsPerHiddenLyr, neuronsPerHiddenLyr));
                }
                //最后一层
                neuronLayers.Add(new SNeuronLayer(numOutPuts, neuronsPerHiddenLyr));
            }
            else
            {
                neuronLayers.Add(new SNeuronLayer(numInputs, numOutPuts));
            }
        }

    }

    /// <summary>
    /// 神经层结构
    /// </summary>
    class SNeuronLayer
    {
        //本层使用的神经细胞数目
        public int NeuronNums;

        public List<SNeuron> Neurons;

        public SNeuronLayer(int numNeurons, int numInputsPerNeruon)
        {
            NeuronNums = numNeurons;
            Neurons = new List<SNeuron>();
            for (int i = 0; i < numNeurons; ++i)
            {
                Neurons.Add(new SNeuron(numInputsPerNeruon));
            }
        }
    }

    /// <summary>
    /// 神经细胞结构
    /// </summary>
    class SNeuron
    {
        //输入个数
        public int InputNums;

        //权重
        public List<double> NeuronWeight;

        public SNeuron(int numInput)
        {
            InputNums = numInput;
            NeuronWeight = new List<double>();
            //针对偏移量+1操作
            for (int i = 0; i < numInput + 1; ++i)
            {
                //随机权重
                NeuronWeight.Add(MathTools.RandomClamped());
            }
        }
    }
    #endregion

}