using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Radar
{
    /// <summary>
    /// 控制类
    /// </summary>
    public class RadarGeneManager : SingletonMono<RadarGeneManager>
    {

        [Header("遗传参数")]
        public int PopulationSize = 20;//种群数量
        public float CrossRate = 0.6f;//交叉率
        public float MutationRate = 0.1f;//突变率
        public float DurationTime = 20f;//多久进化一次
        [Header("神经网络参数")]
        public int InputsNums = 4;//输入参数个数
        public int OutPutsNums = 2;//输出参数个数
        public int HiddenLayerNums = 3;//中间层个数

        [Header("其他")]
        public float ScaleTime = 1;
        public Slider SliderShow;

        private float perturbatlin = 0.1f;//突变扰动参数
        private bool isRun = false;
        private float evolveTimes = 0;

        void Start()
        {
            SliderShow.maxValue = DurationTime;
            RadarTankManager.instance.Begin(PopulationSize, InputsNums, OutPutsNums, HiddenLayerNums);
            Invoke("begin", 0.5f);
        }

        void begin()
        {
            isRun = true;
        }

        float countTime = 0;
        private void Update()
        {
            Time.timeScale = ScaleTime;
            if (isRun)
            {
                SliderShow.value = countTime;
                RadarTankManager.instance.TanksController();
                countTime += Time.deltaTime;
                if (countTime > DurationTime)
                {
                    countTime = 0;
                    evolve();
                }
            }
        }

        /// <summary>
        /// 进化
        /// </summary>
        void evolve()
        {
            evolveTimes++;
            Debug.Log("进化次数:" + evolveTimes);
            isRun = false;
            //上一代的智能
            List<RadarAgent> agents = RadarTankManager.instance.GetAgents();

            RadarAgent dad = null;
            RadarAgent mum = null;

            //精英选择
            //RadarAgent dad = agents[0];
            //RadarAgent mum = agents[1];

            //轮赌盘
            getParent(ref dad, agents);
            getParent(ref mum, agents);

            //下一代的智能
            List<RadarAgent> newAgents = new List<RadarAgent>();

            //交叉与变异
            cross(dad, mum, ref newAgents);
            for (int i = 0; i < newAgents.Count; i++) {
                newAgents[i].Genome.vecWeights = mutate(newAgents[i].Genome.vecWeights);
                newAgents[i].RefreshNeuronNet();
            }
            //mutate(ref newAgents);

            RadarTankManager.instance.ReStart(newAgents);

            isRun = true;
        }

        /// <summary>
        /// 赌轮选择
        /// </summary>
        /// <param name="son"></param>
        /// <param name="agents"></param>
        void getParent(ref RadarAgent son,List<RadarAgent> agents) {
            double allScore = 0;
            foreach (RadarAgent a in agents) {
                allScore += a.AgentScore;
            }
            //double avg = allScore / PopulationSize;
            double ranVal = MathTools.Random01() * allScore;
            double sum = 0;
            for (int i = 0; i < agents.Count; i++) {
                sum += agents[i].AgentScore;
                if (sum >= ranVal) {
                    son = agents[i];
                    return;
                }
            }
        }

        /// <summary>
        ///  交叉
        /// </summary>
        void cross(RadarAgent dad, RadarAgent mum, ref List<RadarAgent> newAgents) {

            while (newAgents.Count < PopulationSize) {
                RadarAgent b1 = dad;
                RadarAgent b2 = mum;

                b1.AgentScore = 0;
                b2.AgentScore = 0;

                for (int i = 0; i < b1.WeightsCount; i++) {
                    if (MathTools.Random01() < CrossRate) {
                        b1.Genome[i] = mum.Genome[i];
                        b2.Genome[i] = dad.Genome[i];
                    }
                }

                b1.RefreshNeuronNet();
                b2.RefreshNeuronNet();

                newAgents.Add(b1);
                if(newAgents.Count < PopulationSize)
                    newAgents.Add(b2);
            }

            //for (int i = 0; i < newAgents.Count; i+=2) {

            //    RadarAgent baby1 = newAgents[i];
            //    RadarAgent baby2 = newAgents[i+1];

            //    baby1.AgentScore = 0;
            //    baby2.AgentScore = 0;
            //    baby1.Genome.vecWeights = dad.Genome.vecWeights;
            //    baby2.Genome.vecWeights = mum.Genome.vecWeights;

            //    for (int j = 0; j < dad.WeightsCount; j++) {
            //        if (MathTools.Random01() < CrossRate) {
            //            double temp = baby2.Genome[j];
            //            baby1.Genome[j] = baby2.Genome[j];
            //            baby2.Genome[j] = temp;
            //        }
            //    }
            //    baby1.RefreshNeuronNet();
            //    baby2.RefreshNeuronNet();

            //    newAgents[i] = baby1;
            //    newAgents[i + 1] = baby2;
            //}

        }

        List<double> mutate(List<double> vec)
        {
            for (int i = 0; i < vec.Count; i++) {
                if (Random01() < MutationRate) {
                    vec[i] += RandomClamped();
                }
            }
            return vec;
        }

        public float Random01()
        {
            return Random.Range(0f, 1f);
        }

        public float RandomClamped()
        {
            return Random.Range(-1f, 1f);
        }

        /// <summary>
        /// 突变函数
        /// </summary>
        //void mutate(ref List<RadarAgent> newAgents) {
        //    for (int i = 0; i < newAgents.Count; i++) {
        //        for (int j = 0; j < newAgents[i].WeightsCount; j++)
        //        {
        //            if (MathTools.Random01() < MutationRate)
        //            {
        //                double ran = MathTools.RandomClamped() * perturbatlin;
        //                newAgents[i].Genome[j] += ran;
        //            }
        //        }
        //        newAgents[i].RefreshNeuronNet();
        //    } 
        //}

        [ContextMenu("Test")]
        public void test()
        {

        }
    }
}