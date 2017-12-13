using UnityEngine;

public class Bullet : MonoBehaviour {

    [Header("子弹射速")]
    public float ShootSpeed = 100.0f;

    Rigidbody m_rigibody;

    private void Awake()
    {
        m_rigibody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Invoke("destory", 3);
    }

    public void Shoot(Vector3 _dir) {
        m_rigibody.AddForce(_dir * ShootSpeed);
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
