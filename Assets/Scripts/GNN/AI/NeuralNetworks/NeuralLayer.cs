using System;
public class NeuralLayer{

    #region 属性
    private static Random randomizer = new Random();

    
    public delegate double ActivationFunction(double xValue);
    /// <summary>
    /// 激活函数
    /// </summary>
    /// <param name="xValue"></param>
    /// <returns></returns>
    public ActivationFunction NeuronActivationFunction = MathHelper.SigmoidFunction;

    /// <summary>
    /// 节点数量
    /// </summary>
    public uint NeuronCount
    {
        get;
        private set;
    }

    /// <summary>
    /// 链接的下一层的节点数量
    /// </summary>
    public uint OutputCount
    {
        get;
        private set;
    }

    /// <summary>
    /// 权重矩阵
    /// </summary>
    public double[,] Weights
    {
        get;
        private set;
    }

    #endregion

    #region 方法

    public NeuralLayer(uint nodeCount, uint outputCount) {
        this.NeuronCount = nodeCount;
        this.OutputCount = outputCount;
        //初始化权重矩阵
        Weights = new double[nodeCount + 1, outputCount];
    }

    /// <summary>
    /// 使用权重矩阵对输入做处理
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public double[] ProcessInput(double[] inputs) {
        if (inputs.Length != NeuronCount)
            throw new ArgumentException("Given xValues do not match layer input count.");

        double[] sums = new double[OutputCount];
        //加权偏差
        double[] biasedInputs = new double[NeuronCount + 1];
        inputs.CopyTo(biasedInputs, 0);
        biasedInputs[inputs.Length] = 1.0;

        //矩阵乘法
        for (int j = 0; j < Weights.GetLength(1); j++) {
            for (int i = 0; i < Weights.GetLength(0); i++) {
                sums[j] += biasedInputs[i] * Weights[i, j];
            }
        }

        //激活
        if (NeuronActivationFunction != null) {
            for (int i = 0; i < sums.Length; i++) {
                sums[i] = NeuronActivationFunction(sums[i]);
            }
        }

        return sums;
    }

    /// <summary>
    /// 数据深拷贝
    /// </summary>
    /// <returns></returns>
    public NeuralLayer DeepCopy() {
        //Copy weights
        double[,] copiedWeights = new double[this.Weights.GetLength(0), this.Weights.GetLength(1)];

        for (int x = 0; x < this.Weights.GetLength(0); x++)
            for (int y = 0; y < this.Weights.GetLength(1); y++)
                copiedWeights[x, y] = this.Weights[x, y];

        //Create copy
        NeuralLayer newLayer = new NeuralLayer(this.NeuronCount, this.OutputCount);
        newLayer.Weights = copiedWeights;
        newLayer.NeuronActivationFunction = this.NeuronActivationFunction;

        return newLayer;
    }

    /// <summary>
    /// 完全随机权重举证
    /// </summary>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public void SetRandomWeights(double minValue, double maxValue)
    {
        double range = Math.Abs(minValue - maxValue);
        for (int i = 0; i < Weights.GetLength(0); i++)
            for (int j = 0; j < Weights.GetLength(1); j++)
                Weights[i, j] = minValue + (randomizer.NextDouble() * range); //random double between minValue and maxValue
    }

    /// <summary>
    /// 返回表示该层的连接权重的字符串。
    /// </summary>
    public override string ToString()
    {
        string output = "";

        for (int x = 0; x < Weights.GetLength(0); x++)
        {
            for (int y = 0; y < Weights.GetLength(1); y++)
                output += "[" + x + "," + y + "]: " + Weights[x, y];

            output += "\n";
        }

        return output;
    }

    #endregion

}
