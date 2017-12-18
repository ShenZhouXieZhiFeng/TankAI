using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radar
{
    public class RadarTank : MonoBehaviour
    {

        public float MaxSpeed = 10;
        public float MaxTorque = 20;
        public bool IsUserController = false;
        public int Score = 0;
        public float CheckDis = 10;
        public LayerMask MineLayer;

        [HideInInspector]
        public RadarAgent Agent;

        Rigidbody _rig;

        void Start()
        {
            _rig = GetComponent<Rigidbody>();
        }

        void Update()
        {
            getNearestMineAngle();
            if (IsUserController)
            {
                float[] input = new float[2];

                input[0] = Input.GetAxis("Horizontal");
                input[1] = Input.GetAxis("Vertical");

                control(input);
            }
            else
            {
                float[] output = new float[4];
                output[0] = _rig.velocity.magnitude;
                output[1] = _rig.angularVelocity.magnitude;
            }
        }

        // 获取扫描范围内最近的地雷,并计算角度
        float getNearestMineAngle()
        {
            float res = 0;
            List<Collider> mines = new List<Collider>(Physics.OverlapSphere(transform.position, CheckDis, MineLayer));
            if (mines != null && mines.Count != 0)
            {
                mines.Sort((Collider x, Collider y) =>
                {
                    return Vector3.Distance(x.transform.position, transform.position).CompareTo(Vector3.Distance(y.transform.position, transform.position));
                });
                tragetPos = mines[0].transform.position;
                Vector3 mineDir = mines[0].transform.position - transform.position;
                res = Vector3.SignedAngle(mineDir, transform.forward, transform.up);
                //res = Vector3.Dot(transform.forward, mineDir.normalized);
            }
            else
            {
                tragetPos = Vector3.zero;
            }
            return res;
        }

        Vector3 tragetPos = Vector3.zero;
        private void OnDrawGizmos()
        {
            if (tragetPos != Vector3.zero)
                Gizmos.DrawLine(transform.position, tragetPos);
        }

        void control(float[] input)
        {
            float h = (float)input[0];
            float v = (float)input[1];

            _rig.velocity = transform.forward * v * MaxSpeed;
            _rig.angularVelocity = transform.up * h * MaxTorque;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Mine"))
            {
                Score++;
                Destroy(collision.gameObject);
                MinesManager.Instance.checkMineNums();
            }
        }

        [ContextMenu("Test")]
        public void Test()
        {
            Debug.Log(getNearestMineAngle());
        }

    }
}
