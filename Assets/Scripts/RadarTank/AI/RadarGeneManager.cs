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

        private float perturbatlin = 0.3f;//突变扰动参数
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

            //精英选择
            RadarAgent dad = agents[0];
            RadarAgent mum = agents[1];

            //下一代的智能
            List<RadarAgent> newAgents = agents;

            //交叉与变异
            cross(dad, mum, ref newAgents);
            mutate(ref newAgents);

            RadarTankManager.instance.ReStart(newAgents);

            isRun = true;
        }

        /// <summary>
        ///  交叉
        /// </summary>
        void cross(RadarAgent dad, RadarAgent mum, ref List<RadarAgent> newAgents) {
            //种群如果为奇数会出问题
            for (int i = 0; i < newAgents.Count; i+=2) {

                RadarAgent baby1 = newAgents[i];
                RadarAgent baby2 = newAgents[i+1];

                baby1.AgentScore = 0;
                baby2.AgentScore = 0;
                baby1.Genome.vecWeights = dad.Genome.vecWeights;
                baby2.Genome.vecWeights = mum.Genome.vecWeights;

                for (int j = 0; j < dad.WeightsCount; j++) {
                    if (MathTools.Random01() < CrossRate) {
                        double temp = baby2.Genome[j];
                        baby1.Genome[j] = baby2.Genome[j];
                        baby2.Genome[j] = temp;
                    }
                }
                baby1.RefreshNeuronNet();
                baby2.RefreshNeuronNet();

                newAgents[i] = baby1;
                newAgents[i + 1] = baby2;
            }

        }

        /// <summary>
        /// 突变函数
        /// </summary>
        void mutate(ref List<RadarAgent> newAgents) {
            for (int i = 0; i < newAgents.Count; i++) {
                for (int j = 0; j < newAgents[i].Genome.vecWeights.Count; j++)
                {
                    if (MathTools.Random01() < MutationRate)
                    {
                        newAgents[i].Genome[j] += (MathTools.Random01() * perturbatlin);
                    }
                }
                newAgents[i].RefreshNeuronNet();
            } 
            
        }

        [ContextMenu("Test")]
        public void test()
        {
            //Debug.Log(randomFloat());
        }

        float randomFloat()
        {
            return Random.Range(0f, 1f);
        }
    }
}