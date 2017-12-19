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
        public float CheckDis = 10;
        public LayerMask MineLayer;

        public List<double> ShowWeights;

        Rigidbody _rig;

        public Agent Agent
        {
            get;
            set;
        }

        void Start()
        {
            _rig = GetComponent<Rigidbody>();
        }

        public void reStart() {

        }

        public void Stop() {

        }

        public void ApplyControl()
        {
            float[] outputs = new float[4];
            outputs[0] = _rig.velocity.magnitude/MaxSpeed;
            outputs[1] = _rig.angularVelocity.magnitude/MaxTorque;
            outputs[2] = transform.forward.magnitude;
            outputs[3] = getNearestMineAngle();

            float[] input = Agent.FNN.ProcessInputs(outputs);

            float h = (float)input[0];
            float v = (float)input[1];

            _rig.velocity = transform.forward * v * MaxSpeed;
            _rig.angularVelocity = transform.up * h * MaxTorque;
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
                //res = Vector3.SignedAngle(mineDir, transform.forward, transform.up);
                float angle = Vector3.Dot(transform.forward, mineDir);
                //顺逆时针
                int sign = MathTools.ReturenSign(transform.forward, mineDir);
                res = sign * angle;
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

        /// <summary>
        /// 更新分数
        /// </summary>
        void updateScore()
        {
            Agent.Genotype.Evaluation++;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Mine"))
            {
                updateScore();
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
