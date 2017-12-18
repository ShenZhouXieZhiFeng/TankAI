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
        List<RadarTank> tanks = new List<RadarTank>();

        void Start()
        {
            RadarTankPrefab.SetActive(false);
        }

        public void CreateTanks(int length)
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

        [ContextMenu("ResetAllTanks")]
        public void ResetAllTanks()
        {
            for (int i = 0; i < tanks.Count; i++)
            {
                tanks[i].Score = 0;
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