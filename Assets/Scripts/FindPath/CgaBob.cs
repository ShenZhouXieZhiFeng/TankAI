using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 遗传算法类
/// </summary>
public class CgaBob : MonoBehaviour {

    [Header("交换概率")]
    public double mCrossOverRate = 0.7;
    [Header("突变概率")]
    public double mMutationRate = 0.01;
    [Header("种群数量")]
    public int mPopSize = 10;
    [Header("最高遗传次数")]
    public int totalGeneSize = 50;
    [Header("染色体长度")]
    public int mChromoLength = 70;//每个染色体包含的bits数量
    [Header("测试速率")]
    public float TestSpeed = 1.0f;
    [Header("两次之间遗传之间的延时")]
    public float GeneDealy = 1.0f;

    private int mCurrentGeneCount = 0;//当前遗传次数
    private int mGeneLength = 2;//每个基因的bits数量

    private int mFittestGenome;
    private double mBestFitNessScore;
    private double mTotalFittnessScore;
    private int mGeneration;

    private List<Sgenome> mGenomes = new List<Sgenome>();

    PathController _path;

    public int MCurrentGeneCount
    {
        get
        {
            return mCurrentGeneCount;
        }

        set
        {
            mCurrentGeneCount = value;
            _path.SetGeneCount(value);
        }
    }

    private void Start()
    {
        _path = GetComponent<PathController>();
        MCurrentGeneCount = 0;
        CreateStartPopulation();
        Run();
    }

    /// <summary>
    /// 创建初始种群
    /// </summary>
    void CreateStartPopulation()
    {
        mGeneration = 0;
        mGenomes.Clear();
        for (int i = 0; i < mPopSize; i++) {
            mGenomes.Add(new Sgenome(mChromoLength));
        }
        mFittestGenome = 0;
        mBestFitNessScore = 0;
        mTotalFittnessScore = 0;
    }
    /// <summary>
    /// 开始执行
    /// </summary>
    void Run() {
        StartCoroutine(testRoute());
    }
    /// <summary>
    /// 遗传
    /// </summary>
    void Epoch()
    {
        if (MCurrentGeneCount > totalGeneSize)
            return;
        MCurrentGeneCount++;
        UpdateGenome();
    }

    /// <summary>
    /// 测试路径，并计算适应性
    /// </summary>
    /// <returns></returns>
    IEnumerator testRoute() {
        yield return new WaitForSeconds(GeneDealy);
        for (int i = 0; i < mGenomes.Count; i++)
        {
            Sgenome gene = mGenomes[i];
            List<int> cmd = decode(gene.vecBits);
            gene.dFitness = _path.TestRoute(cmd);
            yield return new WaitForSeconds(TestSpeed);
        }
        Epoch();
        yield return 0;
    }

    /// <summary>
    /// 更新进化，产生新的种群
    /// </summary>
    void UpdateGenome() {
        //根据适应性排序
        mGenomes.Sort((Sgenome x, Sgenome y)=> {
            return y.dFitness.CompareTo(x.dFitness);
        });
        //使用精英选择的方式，取适应性最高的前2(还有一种轮赌盘的选择方式)
        Sgenome dad = mGenomes[0];
        Sgenome mum = mGenomes[1];

        Debug.Log("当前最高是适应性:" + dad.dFitness);
        if (dad.dFitness == 1)
        {
            Debug.Log("Find Path");
            return;
        }

        mGenomes.Clear();
        while (mGenomes.Count < mPopSize) {

            Sgenome baby1 = new Sgenome();
            Sgenome baby2 = new Sgenome();
            Crossover(mum.vecBits, dad.vecBits, baby1.vecBits, baby2.vecBits);

            Mutate(baby1.vecBits);
            Mutate(baby2.vecBits);

            mGenomes.Add(baby1);
            if (mGenomes.Count < mPopSize)
                mGenomes.Add(baby2);
        }
        StartCoroutine(testRoute());
    }

    /// <summary>
    /// 交换
    /// </summary>
    void Crossover(List<int> mum, List<int> dad, List<int> baby1, List<int> baby2)
    {
        if (RandomFloat() > mCrossOverRate) {
            baby1 = mum;
            baby2 = dad;
            return;
        }
        //确定一个交叉点
        int cp = Random.Range(0, mChromoLength - 1);

        for (int i = 0; i < cp; ++i) {
            baby1.Add(mum[i]);
            baby2.Add(dad[i]);
        }
        for (int i = cp; i < mum.Count; i++) {
            baby1.Add(dad[i]);
            baby2.Add(mum[i]);
        }
    }

    /// <summary>
    /// 突变
    /// </summary>
    void Mutate(List<int> vecBits)
    {
        for (int i = 0; i < vecBits.Count; i++) {
            if (RandomFloat() < mMutationRate) {
                vecBits[i] = vecBits[i] == 1 ? 0 : 1;
            }
        }
    }

    /// <summary>
    /// 返回0-1之前的随机float值
    /// </summary>
    /// <returns></returns>
    float RandomFloat()
    {
        return Random.Range(0f, 1f);
    }
   
    /// <summary>
    /// 将二进制解码成十进制命令
    /// 0=North, 1=South, 2=East, 3=West
    /// </summary>
    List<int> decode(List<int> bits)
    {
        List<int> res = new List<int>();
        for(int gene = 0; gene < bits.Count; gene+=mGeneLength)
        {
            List<int> bit = new List<int>();
            for (int i = 0; i < mGeneLength; i++) {
                bit.Add(bits[gene + i]);
            }
            res.Add(bin2Int(bit));
        }
        return res;
    }
    int bin2Int(List<int> bit) {
        int val = 0;
        int mul = 1;
        for (int i = bit.Count; i > 0 ; i --) {
            val += bit[i - 1] * mul;
            mul *= 2;
        }
        return val;
    }
    
}

/// <summary>
/// 基因
/// </summary>
public class Sgenome
{
    public List<int> vecBits = new List<int>();
    public double dFitness;

    public Sgenome() {
        vecBits.Clear();
        dFitness = 0;
    }

    public Sgenome(int num) {
        dFitness = 0;
        for (int i = 0; i < num; ++i) {
            vecBits.Add(UnityEngine.Random.Range(0, 2));
        }
    }
}
