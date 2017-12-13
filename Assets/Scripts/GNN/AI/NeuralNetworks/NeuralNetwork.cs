using System;
/// <summary>
/// 神经网络
/// </summary>
public class NeuralNetwork{

    #region 属性

    public NeuralLayer[] Layers
    {
        get;
        private set;
    }

    /// <summary>
    /// 各层的节点数量矩阵
    /// [5,4,3,2]:5个输入，2个输出
    /// </summary>
    public uint[] Topology
    {
        get;
        private set;
    }

    /// <summary>
    /// 权重矩阵的数量
    /// </summary>
    public int WeightCount
    {
        get;
        private set;
    }

    #endregion

    #region 方法

    public NeuralNetwork(params uint[] topology) {
        this.Topology = topology;

        WeightCount = 0;
        for (int i = 0; i < topology.Length - 1; i++) {
            WeightCount += (int)((topology[i] + 1) * topology[i + 1]); // + 1 for bias node（+1偏置节点）
        }

        Layers = new NeuralLayer[topology.Length - 1];
        for (int i = 0; i < Layers.Length; i++)
            Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
    }

    public double[] ProcessInputs(double[] inputs) {
        if(inputs.Length != Layers[0].NeuronCount)
            throw new ArgumentException("Given inputs do not match network input amount.");

        double[] outs = inputs;
        foreach (NeuralLayer lays in Layers) {
            outs = lays.ProcessInput(outs);
        }

        return outs;
    }

    /// <summary>
    /// 返回一个以层序表示这个网络的字符串。
    /// </summary>
    public override string ToString()
    {
        string output = "";

        for (int i = 0; i < Layers.Length; i++)
            output += "Layer " + i + ":\n" + Layers[i].ToString();

        return output;
    }

    #endregion

}
