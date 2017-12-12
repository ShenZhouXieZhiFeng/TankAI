using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    public float maxSpeed;
    public float maxTorque;

    public float shootCoolDown;
    public Transform shootPoint;
    public GameController bullet;

    Rigidbody m_rigidbody;

	void Start () {
        m_rigidbody = GetComponent<Rigidbody>();
    }
	
	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        SetMove(v);
        SetRotate(h);
	}

    public void SetMove(float dir) {
        m_rigidbody.velocity = transform.forward * maxSpeed * dir;
    }

    public void SetRotate(float dir) {
        m_rigidbody.angularVelocity = transform.up * maxTorque * dir;
    }

}
