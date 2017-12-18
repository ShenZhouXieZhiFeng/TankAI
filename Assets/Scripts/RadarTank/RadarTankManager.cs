using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radar
{
    public class RadarTankManager : SingletonMono<RadarTankManager>
    {
        public GameObject RadarTankPrefab;
        public Vector4 SpawnDis;

        private int _tankIndex;
        public List<RadarTank> tanks = new List<RadarTank>();

        void Start()
        {
            RadarTankPrefab.SetActive(false);
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="length">种群大小</param>
        /// <param name="intputs">输入参数个数</param>
        /// <param name="outputs">输出参数个数</param>
        /// <param name="hiddenLyeNum">隐藏层数量</param>
        public void Begin(int length,int intputs,int outputs,int hiddenLyeNum) {
            CreateTanks(length);
            initAgent(intputs, outputs, hiddenLyeNum);
        }

        /// <summary>
        /// 创建tank
        /// </summary>
        /// <param name="length"></param>
        void CreateTanks(int length)
        {
            for (int i = 0; i < length; i++)
            {
                GameObject tank = Instantiate(RadarTankPrefab);
                tank.transform.parent = transform;
                tank.transform.position = RandomPosition();
                tank.name += _tankIndex++;
                tanks.Add(tank.GetComponent<RadarTank>());
                tank.SetActive(true);
            }
        }

        /// <summary>
        /// 初始化tank智能
        /// </summary>
        void initAgent(int intputs, int outputs, int hiddenLyeNum) {
            for (int i = 0; i < tanks.Count; i++) {
                tanks[i].Agent = new RadarAgent(intputs, outputs, hiddenLyeNum);
            }
        }

        /// <summary>
        /// 制动所有tank
        /// </summary>
        public void TanksController() {
            foreach (RadarTank tank in tanks) {
                tank.ApplyControl();
            }
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void ReStart(List<RadarAgent> agents) {
            ResetTanksAgent(agents);
            ResetAllTanks();
        }
        /// <summary>
        /// 获取所有的坦克智能
        /// </summary>
        /// <returns></returns>
        public List<RadarAgent> GetAgents() {
            List<RadarAgent> agents = new List<RadarAgent>();
            for (int i = 0; i < tanks.Count; i++) {
                agents.Add(tanks[i].Agent);
            }
            agents.Sort((RadarAgent x, RadarAgent y) => {
                return y.AgentScore.CompareTo(x.AgentScore);
            });
            return agents;
        }
        
        /// <summary>
        /// 重置所有tank的智能
        /// </summary>
        void ResetTanksAgent(List<RadarAgent> agents) {
            if (agents.Count != tanks.Count)
            {
                throw new System.Exception("实体和智能数量不一致");
            }
            for (int i = 0; i < tanks.Count; i++) {
                tanks[i].Agent = agents[i];
            }
        }

        /// <summary>
        /// 随机tank位置
        /// </summary>
        void ResetAllTanks()
        {
            for (int i = 0; i < tanks.Count; i++)
            {
                tanks[i].transform.position = RandomPosition();
            }
        }

        Vector3 RandomPosition()
        {
            float x = Random.Range(SpawnDis.x, SpawnDis.y);
            float y = RadarTankPrefab.transform.position.y;
            float z = Random.Range(SpawnDis.z, SpawnDis.w);
            return new Vector3(x, y, z);
        }
    }
}