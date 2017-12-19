using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radar
{
    public class RadarTankManager : SingletonMono<RadarTankManager>
    {
        #region 属性
        public float maxSpawnX;
        public float minSpawnX;
        public float maxSpawZ;
        public float minSpawZ;

        public RadarTank PrototypeTank;

        private List<RadarTank> tanks = new List<RadarTank>();

        public int TankCount
        {
            get { return tanks.Count; }
        }
        #endregion

        #region 方法
        void Start()
        {
            PrototypeTank.gameObject.SetActive(false);
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void Restart()
        {
            foreach (RadarTank tank in tanks)
            {
                randomSpawn(tank.transform);
                tank.reStart();
            }
        }

        public void TrainTanks()
        {
            foreach (RadarTank c in tanks)
            {
                c.ApplyControl();
            }
        }

        public void SetTankAmount(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount may not be less than zero.");
            if (amount == TankCount) return;
            if (amount > tanks.Count)
            {
                for (int toBeAdded = amount - tanks.Count; toBeAdded > 0; toBeAdded--)
                {
                    GameObject carCopy = Instantiate(PrototypeTank.gameObject);
                    carCopy.transform.parent = transform;
                    RadarTank controllerCopy = carCopy.GetComponent<RadarTank>();
                    randomSpawn(carCopy.transform);
                    tanks.Add(controllerCopy);
                    carCopy.SetActive(true);
                }
            }
        }

        public void Prinf(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// 在一定范围内随机位置
        /// </summary>
        /// <param name="tr"></param>
        void randomSpawn(Transform tr)
        {
            float x = UnityEngine.Random.Range(minSpawnX, maxSpawnX);
            float z = UnityEngine.Random.Range(minSpawZ, maxSpawZ);
            tr.position = new Vector3(x, PrototypeTank.transform.position.y, z);
        }

        public IEnumerator<RadarTank> GetTankEnumerator()
        {
            for (int i = 0; i < tanks.Count; i++)
            {
                yield return tanks[i];
            }
        }

        #endregion
    }
}