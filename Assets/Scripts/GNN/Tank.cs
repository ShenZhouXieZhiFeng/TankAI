using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tank类
/// </summary>
public class Tank : MonoBehaviour {

    #region 属性
    //[Header("最大速度")]
    private float MaxSpeed = 10;
    //[Header("最大旋转")]
    private float MaxTorque = 30;
    //[Header("射速")]
    private float ShootCoolDown = 2;
    //[Header("最大检测距离")]
    private float checkDis = 5;
    [Header("炮口")]
    public Transform ShootPoint;

    //tank渲染
    Transform tankRender;

    /// <summary>
    /// 数据输入
    /// </summary>
    public TankInputModel CurrentInput = new TankInputModel(0, 0, 0);

    /// <summary>
    /// 数据输出
    /// </summary>
    public TankOutPutModel CurrentOutput
    {
        get {
            TankOutPutModel model = new TankOutPutModel();
            model.canShoot = CanShoot ? 1 : 0;
            model.speedPre = m_rigidbody.position.magnitude / MaxSpeed;
            model.rotaPre = m_rigidbody.rotation.eulerAngles.sqrMagnitude / MaxTorque;
            Collider[] res = Physics.OverlapSphere(transform.position, checkDis, 1 << 8);
            if (res.Length == 0)
            {
                model.recentEnemyDis = 0;
                model.checkEnemyNums = 0;
            }
            else {
                model.recentEnemyDis = Vector3.Distance(transform.position, res[0].transform.position);
                model.checkEnemyNums = res.Length;
            }
            return model;
        }
    }

    private bool CanShoot = true;
    Rigidbody m_rigidbody;
    TankController controller;

    #endregion

    #region 函数
    void Start () {
        tankRender = transform.Find("TankRenderers");
        m_rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<TankController>();
    }

    private void FixedUpdate()
    {
        setMove(CurrentInput.moveInput);
        setRotate(CurrentInput.rotaInput);
        shoot(CurrentInput.shootInput);
    }

    public void SetInputs(double[] inputs) {
        CurrentInput.moveInput = (float)inputs[0];
        CurrentInput.rotaInput = (float)inputs[1];
        CurrentInput.shootInput = (float)inputs[2];
    }

    Bullet _bullet = null;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet"))
            return;
        _bullet = other.GetComponent<Bullet>();
        if (_bullet != null && _bullet.Owner != gameObject) {
            takeDamage();
        }
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

    public void Stop() {
        tankRender.gameObject.SetActive(false);
    }
    public void Begin() {
        tankRender.gameObject.SetActive(true);
    }
    void resetShoot() {
        CanShoot = true;
    }

    void takeDamage() {
        controller.Die();
    }
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
    public TankInputModel(float _moveInput,float _rotaInput,float _shootInput) {
        moveInput = _moveInput;
        rotaInput = _rotaInput;
        shootInput = _shootInput;
    }
    public void FormDoubleArray(double[] inputs) {
        if (inputs == null || inputs.Length == 0)
            return;
        moveInput = (float)inputs[0];
        rotaInput = (float)inputs[1];
        shootInput = (float)inputs[2];
    }
}

public struct TankOutPutModel {
    /// <summary>
    /// 当前移动百分比
    /// </summary>
    public float speedPre;
    /// <summary>
    /// 当前旋转百分比
    /// </summary>
    public float rotaPre;
    /// <summary>
    /// 最近敌人距离
    /// </summary>
    public float recentEnemyDis;
    /// <summary>
    /// 检测距离内有多少敌人
    /// </summary>
    public float checkEnemyNums;
    /// <summary>
    /// 是否可以射击
    /// </summary>
    public float canShoot;

    public double[] ToDoubleArray() {
        double[] outs = new double[5];
        outs[0] = speedPre;
        outs[1] = rotaPre;
        outs[2] = recentEnemyDis;
        outs[3] = checkEnemyNums;
        outs[4] = canShoot;
        return outs;
    }
}
