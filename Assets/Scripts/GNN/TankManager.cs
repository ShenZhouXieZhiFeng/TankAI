using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 管理所有坦克
/// </summary>
public class TankManager : SingletonMono<TankManager> {

    #region 属性
    public float maxSpawnX;
    public float minSpawnX;
    public float maxSpawZ;
    public float minSpawZ;

    public TankController PrototypeTank;

    private List<TankController> tanks = new List<TankController>();

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

    void Update()
    {
        //for (int i = 0; i < tanks.Count; i++) {
        //    TankController tank = tanks[i];
        //    if (tank.enabled) {
                
        //    }
        //}
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    public void Restart()
    {

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
                TankController controllerCopy = carCopy.GetComponent<TankController>();
                randomSpawn(carCopy.transform);
                tanks.Add(controllerCopy);
                carCopy.SetActive(true);
            }
        }
        //else if (amount < tanks.Count)
        //{
        //    for (int toBeRemoved = tanks.Count - amount; toBeRemoved > 0; toBeRemoved--)
        //    {
        //        RaceCar last = cars[cars.Count - 1];
        //        cars.RemoveAt(cars.Count - 1);

        //        Destroy(last.Car.gameObject);
        //    }
        //}
    }

    /// <summary>
    /// 在一定范围内随机位置
    /// </summary>
    /// <param name="tr"></param>
    void randomSpawn(Transform tr) {
        float x = Random.Range(minSpawnX, maxSpawnX);
        float z = Random.Range(minSpawZ, minSpawZ);
        tr.position = new Vector3(x, PrototypeTank.transform.position.y, z);
    }

    public IEnumerator<TankController> GetTankEnumerator()
    {
        for (int i = 0; i < tanks.Count; i++) {
            yield return tanks[i];
        }
    }

    #endregion
}
