using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class MoveEntity : MonoBehaviour
    {
        #region Members
        [Header("最大速度")]
        [SerializeField]
        private float maxVelocity;
        [Header("最大动力")]
        [SerializeField]
        private float maxForce;
        [Header("最大旋转速率")]
        [SerializeField]
        private float maxTurnRate;
        [Header("质量")]
        [SerializeField]
        private float mass;
        [Header("警惕距离")]
        [SerializeField]
        private float alertDistance;
        [Header("当前速度")]
        [SerializeField]
        private Vector2 velocity;

        //朝向
        public Vector2 Heading {
            get { return transform.up; }
        }
        public Vector2 Position {
            get { return transform.position; }
        }
        public float Speed {
            get { return velocity.magnitude == 0 ? 1 : velocity.magnitude; }
        }

        public float MaxVelocity {
            get { return maxVelocity; }
            protected set { maxVelocity = value; }
        }

        public float MaxForce {
            get { return maxForce; }
            protected set { maxForce = value; }
        }

        public float MaxTurnRate {
            get { return maxTurnRate; }
            protected set { maxTurnRate = value; }
        }

        public float Mass {
            get { return mass; }
            protected set { mass = value; }
        }

        public float AlertDistance {
            get { return alertDistance; }
            protected set { alertDistance = value; }
        }

        public Vector2 Velocity {
            get { return velocity; }
            protected set { velocity = value; }
        }

        #endregion

        #region Func
        protected void UpdatePosition()
        {
            Vector2 move = Velocity * Time.deltaTime;
            Vector3 posOffset = new Vector3(move.x,move.y,0);
            transform.position += posOffset;
        }
        protected void UpdateRotation()
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, Velocity.normalized);
        }
        /// <summary>
        /// 防止速度超过最大速度
        /// </summary>
        protected void TurnCate(Vector2 _vel)
        {
            if (_vel.magnitude > MaxVelocity)
            {
                Velocity = _vel.normalized * MaxVelocity;
            }
            else
            {
                Velocity = _vel;
            }
        }
        #endregion

    }
}
