using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理进过进化过程
/// </summary>
public class EvolutionManager : SingletonMono<EvolutionManager> {

    #region 属性
    private static System.Random randomizer = new System.Random();

    public float TimePerProcess = 20;
    public Text times;
    public Slider slider;

    // 每一代人口的规模
    [SerializeField]
    private int PopulationSize = 30;

    //遗传算法需要多少代才能重新启动(永远不为0)
    [SerializeField]
    private int RestartAfter = 100;

    //是否使用精英选择或剩余随机抽样
    [SerializeField]
    private bool ElitistSelection = false;

    //代理FNN的拓扑结构
    [SerializeField]
    private uint[] FNNTopology;

    //目前的代理人数量。
    private List<Agent> agents = new List<Agent>();

    /// <summary>
    /// 目前存在的代理数量。
    /// </summary>
    public int AgentsAliveCount
    {
        get;
        private set;
    }

    /// <summary>
    /// 事件发生时所有的代理都已死亡。
    /// </summary>
    public event System.Action AllAgentsDied;

    /// <summary>
    /// 持有遗传算法
    /// </summary>
    private GeneticAlgorithm geneticAlgorithm;

    /// <summary>
    /// 这一代人的年龄。
    /// </summary>
    public uint GenerationCount
    {
        get { return geneticAlgorithm.GenerationCount; }
    }
    #endregion

    #region 方法

    private void Start()
    {
        StartEvolution();
        slider.maxValue = TimePerProcess;
    }

    public void Restart() {
        AllAgentsDied();
    }

    /// <summary>
    /// 开始进化过程
    /// </summary>
    public void StartEvolution()
    {
        //创建神经网络来确定参数计数
        NeuralNetwork nn = new NeuralNetwork(FNNTopology);

        //设置遗传算法
        geneticAlgorithm = new GeneticAlgorithm((uint)nn.WeightCount, (uint)PopulationSize);

        //设定评估人口的方法
        geneticAlgorithm.Evaluation = StartEvaluation;

        //精英采样，设定哪些人口可以遗传到下一代
        if (ElitistSelection)
        {
            //选取当前人口中评价最高的三个进行遗传
            geneticAlgorithm.Selection = GeneticAlgorithm.DefaultSelectionOperator;
            //遗传算法重组方法，插入随机因子
            geneticAlgorithm.Recombination = RandomRecombination;
            //设定种群变异方法
            //将新种群中的所有成员都用默认的概率进行变异，同时将前2个基因型留在列表中。
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        else
        {
            //默认，随机采样
            geneticAlgorithm.Selection = RemainderStochasticSampling;
            geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }

        //当所有的代理都死亡，进行人口评价，遗传计算，生成新人口
        AllAgentsDied += geneticAlgorithm.EvaluationFinished;

        //重新启动逻辑
        if (RestartAfter > 0)
        {
            geneticAlgorithm.TerminationCriterion += CheckGenerationTermination;
            geneticAlgorithm.AlgorithmTerminated += OnGATermination;
        }
        //执行遗传算法
        geneticAlgorithm.Start();
    }

    /// <summary>
    /// 检查是否满足了生成计数的终止条件。
    /// </summary>
    /// <param name="currentPopulation"></param>
    /// <returns></returns>
    private bool CheckGenerationTermination(IEnumerable<Genotype> currentPopulation)
    {
        return geneticAlgorithm.GenerationCount >= RestartAfter;
    }

    /// <summary>
    /// 在基因算法被终止的时候被调用
    /// </summary>
    /// <param name="ga"></param>
    private void OnGATermination(GeneticAlgorithm ga)
    {
        AllAgentsDied -= ga.EvaluationFinished;

        RestartAlgorithm(5.0f);
    }

    //在特定的等待时间之后重新启动算法
    private void RestartAlgorithm(float wait)
    {
        Invoke("StartEvolution", wait);
    }

    /// <summary>
    /// 首先从当前的人口中创建新的代理，然后重新启动跟踪管理器。
    /// </summary>
    /// <param name="currentPopulation"></param>
    private void StartEvaluation(IEnumerable<Genotype> currentPopulation)
    {
        //从上一代的人口中创建新的人口代理
        agents.Clear();
        AgentsAliveCount = 0;
        foreach (Genotype genotype in currentPopulation)
            agents.Add(new Agent(genotype, MathHelper.SoftSignFunction, FNNTopology));
        TankManager.Instance.SetTankAmount(agents.Count);
        IEnumerator<TankController> carsEnum = TankManager.Instance.GetTankEnumerator();
        for (int i = 0; i < agents.Count; i++)
        {
            if (!carsEnum.MoveNext())
            {
                Debug.LogError("Tanks enum ended before agents.");
                break;
            }
            carsEnum.Current.Agent = agents[i];
            AgentsAliveCount++;
            //指定死亡回调
            agents[i].AgentDied += OnAgentDied;
        }
        //重新启动
        TankManager.Instance.Restart();
    }

    // Callback for when an agent died.
    /// <summary>
    /// 当代理死亡时回调。
    /// </summary>
    /// <param name="agent"></param>
    private void OnAgentDied(Agent agent)
    {
        AgentsAliveCount--;
        //当所有的智能都死亡时
        if (AgentsAliveCount == 0 && AllAgentsDied != null)
            AllAgentsDied();
    }

    /// <summary>
    /// 遗传算法的选择算子，使用一种称为剩余随机抽样的方法。
    /// </summary>
    /// <param name="currentPopulation"></param>
    /// <returns></returns>
    private List<Genotype> RemainderStochasticSampling(List<Genotype> currentPopulation)
    {
        List<Genotype> intermediatePopulation = new List<Genotype>();
        //将基因型的整型分为中间型
        //假设当前的人口已经排好序
        foreach (Genotype genotype in currentPopulation)
        {
            if (genotype.Fitness < 1)
                break;
            else
            {
                for (int i = 0; i < (int)genotype.Fitness; i++)
                    intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
            }
        }

        //Put remainder portion of genotypes into intermediatePopulation
        //将基因型的剩余部分放入中位调节中
        foreach (Genotype genotype in currentPopulation)
        {
            float remainder = genotype.Fitness - (int)genotype.Fitness;
            if (randomizer.NextDouble() < remainder)
                intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
        }

        return intermediatePopulation;
    }

    /// <summary>
    /// 遗传算法重组算子，重组中间种群的随机基因型
    /// </summary>
    /// <param name="intermediatePopulation"></param>
    /// <param name="newPopulationSize"></param>
    /// <returns></returns>
    private List<Genotype> RandomRecombination(List<Genotype> intermediatePopulation, uint newPopulationSize)
    {
        //Check arguments
        //检查参数
        if (intermediatePopulation.Count < 2)
            throw new System.ArgumentException("The intermediate population has to be at least of size 2 for this operator.");

        List<Genotype> newPopulation = new List<Genotype>();
        //总是添加最好的两个(未修改的)
        newPopulation.Add(intermediatePopulation[0]);
        newPopulation.Add(intermediatePopulation[1]);


        while (newPopulation.Count < newPopulationSize)
        {
            //得到两个不一样的随机指数
            int randomIndex1 = randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
            do
            {
                randomIndex2 = randomizer.Next(0, intermediatePopulation.Count);
            } while (randomIndex2 == randomIndex1);

            Genotype offspring1, offspring2;
            GeneticAlgorithm.CompleteCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2],
                GeneticAlgorithm.DefCrossSwapProb, out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }

        return newPopulation;
    }

    /// <summary>
    /// 将新种群中的所有成员都用默认的概率进行变异，同时将前2个基因型留在列表中。
    /// </summary>
    /// <param name="newPopulation"></param>
    private void MutateAllButBestTwo(List<Genotype> newPopulation)
    {
        //遍历新基因型
        for (int i = 2; i < newPopulation.Count; i++)
        {
            //根据比较随机值和遗传的突变率进行突变操作
            if (randomizer.NextDouble() < GeneticAlgorithm.DefMutationPerc)
                GeneticAlgorithm.MutateGenotype(newPopulation[i], GeneticAlgorithm.DefMutationProb, GeneticAlgorithm.DefMutationAmount);
        }
    }

    /// <summary>
    /// 使用默认参数对新用户的所有成员进行修改
    /// </summary>
    /// <param name="newPopulation"></param>
    private void MutateAll(List<Genotype> newPopulation)
    {
        foreach (Genotype genotype in newPopulation)
        {
            if (randomizer.NextDouble() < GeneticAlgorithm.DefMutationPerc)
                GeneticAlgorithm.MutateGenotype(genotype, GeneticAlgorithm.DefMutationProb, GeneticAlgorithm.DefMutationAmount);
        }
    }

    #endregion

}
