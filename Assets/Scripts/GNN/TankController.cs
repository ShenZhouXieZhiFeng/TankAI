using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tank控制类
/// </summary>
public class TankController : MonoBehaviour {

    public bool IsUserController = false;

    #region 属性
    /// <summary>
    /// 大脑
    /// </summary>
    public Agent Agent
    {
        get;
        set;
    }

    /// <summary>
    /// 当前评分
    /// </summary>
    public float CurrentScore
    {
        get { return Agent.Genotype.Evaluation; }
        set { Agent.Genotype.Evaluation = value; }
    }

    /// <summary>
    /// 唯一ID
    /// </summary>
    private static int tankId = 0;
    private static int NextID
    {
        get { return tankId++; }
    }

    /// <summary>
    /// 链接到tank实体
    /// </summary>
    public Tank Tank
    {
        get;
        private set;
    }

    public TankInputModel CurrentControllInputs
    {
        get { return Tank.CurrentInput; }
    }

    #endregion

    #region 方法
    private void Awake()
    {
        Tank = GetComponent<Tank>();
    }
    void Start () {
        name = "Tank(" + NextID + ")";
    }
	void Update () {
        if (IsUserController) {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Tank.setMove(v);
            Tank.setRotate(h);
            if (Input.GetKeyDown(KeyCode.J)) {
                Tank.shoot(1);
            }
        }
	}
    private void FixedUpdate()
    {
        //从智能从获取控制输入
        if (!IsUserController)
        {
            //获取传感器的参数,5个传感器的长度
            double[] intputs = Tank.CurrentOutput.ToDoubleArray();
            //神经网络处理，输出新的控制
            double[] controlInputs = Agent.FNN.ProcessInputs(intputs);
            //车辆制动
            Tank.SetInputs(controlInputs);
        }
    }

    /// <summary>
    /// 击杀得分
    /// </summary>
    public void KillSomeOne() {
        CurrentScore++;
    }

    /// <summary>
    /// 重启
    /// </summary>
    public void Restart() {
        Tank.enabled = true;
        Tank.Begin();
        this.enabled = true;
        Agent.Reset();
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public void Die() {
        CurrentScore -= 5;
        this.enabled = false;
        Tank.Stop();
        Tank.enabled = false;
        Agent.Kill();
    }
    #endregion
}
