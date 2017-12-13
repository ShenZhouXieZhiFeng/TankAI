using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    public float MaxSpeed;
    public float MaxTorque;
    public float ShootCoolDown;
    public Transform ShootPoint;

    Rigidbody m_rigidbody;

	void Start () {
        m_rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            shoot();
        }
	}

    public void ApplyBehavior() {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet"))
            return;
        takeDamage();
    }

    void setMove(float dir) {
        m_rigidbody.velocity = transform.forward * MaxSpeed * dir;
    }

    void setRotate(float dir) {
        m_rigidbody.angularVelocity = transform.up * MaxTorque * dir;
    }

    void shoot() {
        BulletPool.Instance.GetBullet(ShootPoint, ShootPoint.forward);
    }

    void takeDamage() {
        gameObject.SetActive(false);
    }

}
