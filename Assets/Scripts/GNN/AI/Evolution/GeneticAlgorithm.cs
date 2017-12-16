/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
using System.Collections.Generic;
#endregion

/// <summary>
/// Class implementing a modified genetic algorithm
/// 实现修改遗传算法的类
/// </summary>
public class GeneticAlgorithm
{
    #region Members
    #region Default Parameters
    /// <summary>
    /// Default min value of inital population parameters.
    /// 默认的最小值的人口参数。
    /// </summary>
    public const float DefInitParamMin = -1.0f;
    /// <summary>
    /// Default max value of initial population parameters.
    /// 初始人口参数的缺省最大值。
    /// </summary>
    public const float DefInitParamMax = 1.0f;

    /// <summary>
    /// Default probability of a parameter being swapped during crossover.
    /// 在交叉过程中交换参数的默认概率。
    /// </summary>
    public const float DefCrossSwapProb = 0.6f;

    /// <summary>
    /// Default probability of a parameter being mutated.
    /// 一个参数被突变的默认概率。
    /// </summary>
    public const float DefMutationProb = 0.3f;
    /// <summary>
    /// Default amount by which parameters may be mutated.
    /// 缺省数量的参数可能会发生突变。
    /// </summary>
    public const float DefMutationAmount = 2.0f;
    /// <summary>
    /// Default percent of genotypes in a new population that are mutated.
    /// 在新种群中，基因型的默认比例是变异的。
    /// </summary>
    public const float DefMutationPerc = 1.0f;
    #endregion

    #region Operator Delegates
    /// <summary>
    /// Method template for methods used to initialise the initial population.
    /// 用于初始化初始人口的方法的方法模板。
    /// </summary>
    /// <param name="initialPopulation">The population to be initialised.</param>
    public delegate void InitialisationOperator(IEnumerable<Genotype> initialPopulation);
    /// <summary>
    /// Method template for methods used to evaluate (or start the evluation process of) the current population.
    /// 用于评估(或启动对当前人口的evluation进程)的方法的方法模板
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    public delegate void EvaluationOperator(IEnumerable<Genotype> currentPopulation);
    /// <summary>
    /// Method template for methods used to calculate the fitness value of each genotype of the current population.
    /// 方法模板用于计算当前种群的每个基因型的适合度值
    /// </summary>
    /// <param name="currentPopulation"></param>
    public delegate void FitnessCalculation(IEnumerable<Genotype> currentPopulation);
    /// <summary>
    /// Method template for methods used to select genotypes of the current population and create the intermediate population.
    /// 方法模板用于选择当前种群的基因型，并创建中间种群。
    /// </summary>
    /// <param name="currentPopulation">The current population,</param>
    /// <returns>The intermediate population.</returns>
    public delegate List<Genotype> SelectionOperator(List<Genotype> currentPopulation);
    /// <summary>
    /// Method template for methods used to recombine the intermediate population to generate a new population.
    /// 方法模板用于重新组合中间人群以生成新的人口。
    /// </summary>
    /// <param name="intermediatePopulation">The intermediate population.</param>
    /// <returns>The new population.</returns>
    public delegate List<Genotype> RecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize);
    /// <summary>
    /// Method template for methods used to mutate the new population.
    /// 用于改变新种群的方法的方法模板。
    /// </summary>
    /// <param name="newPopulation">The mutated new population.</param>
    public delegate void MutationOperator(List<Genotype> newPopulation);
    /// <summary>
    /// Method template for method used to check whether any termination criterion has been met.
    /// 用于检查是否满足终止条件的方法的方法模板。
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    /// <returns>Whether the algorithm shall be terminated.</returns>
    public delegate bool CheckTerminationCriterion(IEnumerable<Genotype> currentPopulation);
    #endregion

    #region Operator Methods
    /// <summary>
    /// Method used to initialise the initial population.
    /// 用来初始化初始种群的方法
    /// </summary>
    public InitialisationOperator InitialisePopulation = DefaultPopulationInitialisation;
    /// <summary>
    /// Method used to evaluate (or start the evaluation process of) the current population.
    /// 方法用于评估(或启动评估过程)当前人口。
    /// </summary>
    public EvaluationOperator Evaluation = AsyncEvaluation;
    /// <summary>
    /// Method used to calculate the fitness value of each genotype of the current population.
    /// 方法用于计算当前种群的每个基因型的适合度值。
    /// </summary>
    public FitnessCalculation FitnessCalculationMethod = DefaultFitnessCalculation;
    /// <summary>
    /// Method used to select genotypes of the current population and create the intermediate population.
    /// 方法用于选择当前种群的基因型，并创造中间种群。
    /// </summary>
    public SelectionOperator Selection = DefaultSelectionOperator;
    /// <summary>
    /// Method used to recombine the intermediate population to generate a new population.
    /// 方法用于重新组合中间人群以产生新的人口。
    /// </summary>
    public RecombinationOperator Recombination = DefaultRecombinationOperator;
    /// <summary>
    /// Method used to mutate the new population.
    /// 方法用于变异新种群。
    /// </summary>
    public MutationOperator Mutation = DefaultMutationOperator;
    /// <summary>
    /// Method used to check whether any termination criterion has been met.
    /// 方法用于检查是否满足了任何终止条件。
    /// </summary>
    public CheckTerminationCriterion TerminationCriterion = null;
    #endregion

    private static Random randomizer = new Random();

    private List<Genotype> currentPopulation;

    /// <summary>
    /// The amount of genotypes in a population.
    /// 种群中基因型的数量。
    /// </summary>
    public uint PopulationSize
    {
        get;
        private set;
    }

    /// <summary>
    /// The amount of generations that have already passed.
    /// 已经通过的几代人的数量。
    /// </summary>
    public uint GenerationCount
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether the current population shall be sorted before calling the termination criterion operator.
    /// 在调用终止标准操作符之前，是否应该对当前的人口进行排序。
    /// </summary>
    public bool SortPopulation
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether the genetic algorithm is currently running.
    /// 遗传算法是否正在运行。
    /// </summary>
    public bool Running
    {
        get;
        private set;
    }

    /// <summary>
    /// Event for when the algorithm is eventually terminated.
    /// 当算法最终被终止的时候。
    /// </summary>
    public event System.Action<GeneticAlgorithm> AlgorithmTerminated;
    /// <summary>
    /// Event for when the algorithm has finished fitness calculation. Given parameter is the
    /// current population sorted by fitness if sorting is enabled (see <see cref="SortPopulation"/>).
    /// 当算法完成了适合度的计算时。给定参数是当前按适合度排序的人群如果排序是启用的
    /// </summary>
    public event System.Action<IEnumerable<Genotype>> FitnessCalculationFinished;

    #endregion

    #region Constructors
    /// <summary>
    /// 初始化一个新的遗传算法实例，用给定参数的基因型来创建给定大小的初始种群。
    /// Initialises a new genetic algorithm instance, creating a initial population of given size with genotype
    /// of given parameter count.
    /// </summary>
    /// <param name="genotypeParamCount">The amount of parameters per genotype.</param>
    /// <param name="populationSize">The size of the population.</param>
    /// <remarks>
    /// The parameters of the genotypes of the inital population are set to the default float value.
    /// In order to initialise a population properly, assign a method to <see cref="InitialisePopulation"/>
    /// and call <see cref="Start"/> to start the genetic algorithm.
    /// </remarks>
    public GeneticAlgorithm(uint genotypeParamCount, uint populationSize)
    {
        this.PopulationSize = populationSize;
        //Initialise empty population
        //初始化空人口,Genotype是每个被用于测试的个体
        currentPopulation = new List<Genotype>((int)populationSize);
        for (int i = 0; i < populationSize; i++)
            currentPopulation.Add(new Genotype(new float[genotypeParamCount]));
        //遗传代数
        GenerationCount = 1;
        //是否进行人口排序
        SortPopulation = true;
        //遗传算法是否正在执行
        Running = false;
    }
    #endregion

    #region Methods
    public void Start()
    {
        Running = true;
        //初始化种群,随机参数
        InitialisePopulation(currentPopulation);
        //评估当前人口,根据之前绑定的评估方法
        Evaluation(currentPopulation);
    }

    /// <summary>
    /// 当所有的智能都死亡
    /// </summary>
    public void EvaluationFinished()
    {
        //Calculate fitness from evaluation
        //计算适应度的评价，针对所有的基因型
        FitnessCalculationMethod(currentPopulation);

        //Sort population if flag was set
        //如果设置了标志，就可以进行排序
        if (SortPopulation)
            currentPopulation.Sort();

        //Check termination criterion
        //检查终止准则，比如遗传次数是否达到最大值
        if (TerminationCriterion != null && TerminationCriterion(currentPopulation))
        {
            //停止传代
            Terminate();
            return;
        }

        //Apply Selection
        List<Genotype> intermediatePopulation = Selection(currentPopulation);

        //Apply Recombination
        //应用复合
        List<Genotype> newPopulation = Recombination(intermediatePopulation, PopulationSize);

        //Apply Mutation
        //应用突变
        Mutation(newPopulation);

        //Set current population to newly generated one and start evaluation again
        //将当前的人口设置为新生成的，并重新开始评估
        currentPopulation = newPopulation;
        GenerationCount++;

        //启动新的遗传
        Evaluation(currentPopulation);
    }

    private void Terminate()
    {
        Running = false;
        if (AlgorithmTerminated != null)
            AlgorithmTerminated(this);
    }

    #region Static Methods
    #region Default Operators
    /// <summary>
    /// 通过将每个参数设置为默认值的一个随机值来初始化种群。
    /// Initialises the population by setting each parameter to a random value in the default range.
    /// </summary>
    /// <param name="population">The population to be initialised.</param>
    public static void DefaultPopulationInitialisation(IEnumerable<Genotype> population)
    {
        //Set parameters to random values in set range
        //将参数设置为设置范围中的随机值
        foreach (Genotype genotype in population)
            genotype.SetRandomParameters(DefInitParamMin, DefInitParamMax);
    }

    public static void AsyncEvaluation(IEnumerable<Genotype> currentPopulation)
    {
        //At this point the async evaluation should be started and after it is finished EvaluationFinished should be called
    }

    /// <summary>
    /// 根据公式计算每个基因型的适应性:健康=评价/平均评估。
    /// Calculates the fitness of each genotype by the formula: fitness = evaluation / averageEvaluation.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    public static void DefaultFitnessCalculation(IEnumerable<Genotype> currentPopulation)
    {
        //First calculate average evaluation of whole population
        uint populationSize = 0;
        float overallEvaluation = 0;
        foreach (Genotype genotype in currentPopulation)
        {
            overallEvaluation += genotype.Evaluation;
            populationSize++;
        }
        //评价评价
        float averageEvaluation = overallEvaluation / populationSize;

        //Now assign fitness with formula fitness = evaluation / averageEvaluation
        //现在用公式的适应性来分配健康度=评估/平均评估
        foreach (Genotype genotype in currentPopulation)
            genotype.Fitness = genotype.Evaluation / averageEvaluation;
    }

    /// <summary>
    /// 只选择当前种群中最好的三种基因型，并将它们复制到中间种群中。
    /// Only selects the best three genotypes of the current population and copies them to the intermediate population.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    /// <returns>The intermediate population.</returns>
    public static List<Genotype> DefaultSelectionOperator(List<Genotype> currentPopulation)
    {
        //直接取健壮性最高的前三个基因型
        List<Genotype> intermediatePopulation = new List<Genotype>();
        intermediatePopulation.Add(currentPopulation[0]);
        intermediatePopulation.Add(currentPopulation[1]);
        intermediatePopulation.Add(currentPopulation[2]);

        TankManager.Instance.Prinf("最高得分为:" + currentPopulation[0].Evaluation);

        return intermediatePopulation;
    }

    /// <summary>
    /// 简单地将第一个基因与第二个基因型的中间人群杂交，直到新种群的数量达到预期的大小。
    /// Simply crosses the first with the second genotype of the intermediate population until the new 
    /// population is of desired size.
    /// </summary>
    /// <param name="intermediatePopulation">The intermediate population that was created from the selection process.</param>
    /// <returns>The new population.</returns>
    public static List<Genotype> DefaultRecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize)
    {
        if (intermediatePopulation.Count < 2) throw new ArgumentException("Intermediate population size must be greater than 2 for this operator.");
        //生成新的基因型
        List<Genotype> newPopulation = new List<Genotype>();
        while (newPopulation.Count < newPopulationSize)
        {
            Genotype offspring1, offspring2;
            //将基因0和1，交换参数，进行叉乘操作，返回两个新的基因型
            CompleteCrossover(intermediatePopulation[0], intermediatePopulation[1], DefCrossSwapProb, out offspring1, out offspring2);
            //将新的基因型进行保存
            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }
        return newPopulation;
    }

    /// <summary>
    /// 简单地用默认突变概率和数量突变每个基因型。
    /// Simply mutates each genotype with the default mutation probability and amount.
    /// </summary>
    /// <param name="newPopulation">The mutated new population.</param>
    public static void DefaultMutationOperator(List<Genotype> newPopulation)
    {
        foreach (Genotype genotype in newPopulation)
        {
            if (randomizer.NextDouble() < DefMutationPerc)
                MutateGenotype(genotype, DefMutationProb, DefMutationAmount);
        }
    }
    #endregion

    #region Recombination Operators
    /// <summary>
    /// 完整的交叉
    /// </summary>
    public static void CompleteCrossover(Genotype parent1, Genotype parent2, float swapChance, out Genotype offspring1, out Genotype offspring2)
    {
        //Initialise new parameter vectors
        //初始化新的参数矩阵
        int parameterCount = parent1.ParameterCount;
        float[] off1Parameters = new float[parameterCount], off2Parameters = new float[parameterCount];

        //Iterate over all parameters randomly swapping
        //遍历所有参数，随机交换
        for (int i = 0; i < parameterCount; i++)
        {
            //如果生成的随机数小于交换参考值，则进行交换
            if (randomizer.Next() < swapChance)
            {
                //Swap parameters
                off1Parameters[i] = parent2[i];
                off2Parameters[i] = parent1[i];
            }
            else
            {
                //不进行交换，直接遗传到子基因
                //Don't swap parameters
                off1Parameters[i] = parent1[i];
                off2Parameters[i] = parent2[i];
            }
        }
        //根据新的参数矩阵生成新的基于型
        offspring1 = new Genotype(off1Parameters);
        offspring2 = new Genotype(off2Parameters);
    }
    #endregion

    #region Mutation Operators
    /// <summary>
    /// 通过在每个参数中增加一个随机值，在每个参数上增加一个随机值，从而改变给定的基因型。
    /// Mutates the given genotype by adding a random value in range [-mutationAmount, mutationAmount] to each parameter with a probability of mutationProb.
    /// </summary>
    /// <param name="genotype">The genotype to be mutated.</param>
    /// <param name="mutationProb">The probability of a parameter being mutated.</param>
    /// <param name="mutationAmount">A parameter may be mutated by an amount in range [-mutationAmount, mutationAmount].</param>
    public static void MutateGenotype(Genotype genotype, float mutationProb, float mutationAmount)
    {
        //mutationProb 一个参数被突变的概率
        //mutationAmount 默认的变异参数
        for (int i = 0; i < genotype.ParameterCount; i++)
        {
            if (randomizer.NextDouble() < mutationProb)
            {
                //Mutate by random amount in range [-mutationAmount, mutationAmoun]
                //控制变异后的参数在一定的量内
                genotype[i] += (float)(randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount);
            }
        }
    }
    #endregion
    #endregion
    #endregion

}
