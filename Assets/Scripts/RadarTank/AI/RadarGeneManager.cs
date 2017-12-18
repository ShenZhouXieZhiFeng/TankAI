using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        void Start()
        {
            RadarTankManager.instance.CreateTanks(PopulationSize);
            InvokeRepeating("evolve", 0, DurationTime);
        }

        /// <summary>
        /// 进化
        /// </summary>
        void evolve()
        {

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