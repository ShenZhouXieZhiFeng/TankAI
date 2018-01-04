using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class Tank : MoveEntity
    {
        #region Members
        GameWorld _gameWorld;
        SteeringBehaviour _steering;
        #endregion

        [Header("测试")]
        public bool CanMove = false;
        public MoveEntity Target;

        void Start()
        {
            _steering = new SteeringBehaviour(this);
        }

        void Update()
        {
            if (CanMove)
            {
                //Vector2 newVel = _steering.Pursute(Target);
                //Vector2 newVel = _steering.Evade(Target);
                Vector2 newVel = _steering.Wander();
                UpdateTank(newVel);
            }
        }

        void UpdateTank(Vector2 _steeringForce)
        {
            Vector2 steeringForce = _steeringForce;
            Vector2 acceleration = steeringForce / Mass;
            Velocity += acceleration * Time.deltaTime;

            TurnCate(Velocity);
            UpdatePosition();
            UpdateRotation();
        }

        private void OnDrawGizmos()
        {
            Vector2 vec2 = Velocity.normalized * AlertDistance;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, AlertDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + vec2.x, transform.position.y + vec2.y, transform.position.z));// Velocity.normalized * 10);
        }
    }
}
