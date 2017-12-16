using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tank类
/// </summary>
public class Tank : MonoBehaviour {

    #region 属性
    [Header("最大速度")]
    public float MaxSpeed = 10;
    [Header("最大旋转")]
    public float MaxTorque = 30;
    [Header("射速")]
    public float ShootCoolDown = 2;
    [Header("最大检测距离")]
    public float checkDis = 5;
    [Header("炮口")]
    public Transform ShootPoint;
    [Header("EnemyMask")]
    public LayerMask enemyMask;

    //tank渲染
    Transform tankRender;
    Collider m_collider;

    /// <summary>
    /// 数据输入
    /// </summary>
    [SerializeField]
    public TankInputModel CurrentInput = new TankInputModel(0, 0, 0);

    public float inSpeed;
    public float inRota;

    /// <summary>
    /// 数据输出
    /// </summary>
    public TankOutPutModel CurrentOutput
    {
        get {
            TankOutPutModel model = new TankOutPutModel();
            Transform closestEnemy = getClosestEnemy();

            model.closestEnemyDic = closestEnemy != null ? Vector3.Distance(transform.position, closestEnemy.position) / checkDis : 1f;
            model.closestEnemyCos = closestEnemy != null ? Vector3.Dot(transform.right, (closestEnemy.position - transform.position).normalized) : 1f;
            model.canShoot = CanShoot ? 1f : 0f;
            model.speed = m_rigidbody.velocity.magnitude / MaxSpeed;
            model.torque = m_rigidbody.angularVelocity.magnitude / MaxTorque;

            return model;
        }
    }

    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    /// <returns></returns>
    Transform getClosestEnemy()
    {
        var cols = new List<Collider>(Physics.OverlapSphere(transform.position, checkDis, enemyMask));
        cols.Remove(m_collider);
        var firstOrDefault = cols.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
        return firstOrDefault != null ? firstOrDefault.transform : null;
    }

    private bool CanShoot = true;
    Rigidbody m_rigidbody;
    TankController controller;

    #endregion

    #region 函数

    private void Awake()
    {
        tankRender = transform.Find("TankRenderers");
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<TankController>();
    }

    public void SetInputs(float[] inputs) {
        CurrentInput.moveInput = inputs[0];
        CurrentInput.rotaInput = inputs[1];
        CurrentInput.shootInput = inputs[2];
        inSpeed = CurrentInput.moveInput;
        inRota = CurrentInput.rotaInput;

        setMove(CurrentInput.moveInput);
        setRotate(CurrentInput.rotaInput);
        shoot(CurrentInput.shootInput);
    }

    public void setMove(float dir) {
        m_rigidbody.velocity = transform.forward * MaxSpeed * dir;
    }

    public void setRotate(float dir) {
        m_rigidbody.angularVelocity = transform.up * MaxTorque * dir;
    }

    public void shoot(float val) {
        if (!CanShoot)
            return;
        if (val < 0)
            return;
        CanShoot = false;
        Invoke("resetShoot", ShootCoolDown);
        BulletPool.Instance.GetBullet(gameObject, ShootPoint, ShootPoint.forward);
    }
    void resetShoot()
    {
        CanShoot = true;
    }

    public void Stop() {
        m_rigidbody.Sleep();
        m_collider.enabled = false;
        tankRender.gameObject.SetActive(false);
    }
    public void Begin() {
        m_rigidbody.WakeUp();
        m_collider.enabled = true;
        tankRender.gameObject.SetActive(true);
    }

    //public void takeDamage() {
    //    controller.Die();
    //}
    #endregion
    
}

public struct TankInputModel
{
    /// <summary>
    /// 移动输入
    /// </summary>
    public float moveInput;
    /// <summary>
    /// 旋转输入
    /// </summary>
    public float rotaInput;
    /// <summary>
    /// 发射输入
    /// </summary>
    public float shootInput;
    public TankInputModel(float _moveInput, float _rotaInput, float _shootInput) {
        moveInput = _moveInput;
        rotaInput = _rotaInput;
        shootInput = _shootInput;
    }
    public void FormDoubleArray(float[] inputs) {
        if (inputs == null || inputs.Length == 0)
            return;
        moveInput = inputs[0];
        rotaInput = inputs[1];
        shootInput = inputs[2];
    }
}

public struct TankOutPutModel {
    /// <summary>
    /// 当前移动百分比
    /// </summary>
    public float closestEnemyDic;
    /// <summary>
    /// 当前旋转百分比
    /// </summary>
    public float closestEnemyCos;
    /// <summary>
    /// 最近敌人距离
    /// </summary>
    public float canShoot;
    /// <summary>
    /// 检测距离内有多少敌人
    /// </summary>
    public float speed;
    /// <summary>
    /// 是否可以射击
    /// </summary>
    public float torque;

    public float[] ToDoubleArray() {
        float[] outs = new float[5];
        outs[0] = closestEnemyDic;
        outs[1] = closestEnemyCos;
        outs[2] = canShoot;
        outs[3] = speed;
        outs[4] = torque;
        return outs;
    }
}
