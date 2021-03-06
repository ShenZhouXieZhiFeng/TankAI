﻿using UnityEngine;

public class Bullet : MonoBehaviour {

    [Header("子弹射速")]
    public float ShootSpeed = 100.0f;
    [Header("自销毁时间")]
    public float DesoryTime = 2;

    public float ShootDis = 5;

    public LayerMask HitLayerMask;

    [HideInInspector]
    public GameObject Owner;

    Rigidbody m_rigibody;

    private void Awake()
    {
        m_rigibody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Invoke("destory", DesoryTime);
    }
    public void Shoot(Vector3 _dir) {
        m_rigibody.AddForce(_dir * ShootSpeed);
        Ray ray = new Ray(transform.position, _dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, ShootDis,HitLayerMask)) {
            TankController con = hit.transform.GetComponent<TankController>();
            con.Die();
            Owner.GetComponent<TankController>().KillSomeOne();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Tank") && !other.CompareTag("Wall"))
            return;
        destory();
    }

    void destory() {
        BulletPool.Instance.PushBullet(this);
    }

}
