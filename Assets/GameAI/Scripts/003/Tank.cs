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

        void Start()
        {
            _steering = new SteeringBehaviour(this);
        }

        void Update()
        {
            if (CanMove)
            {
                //if (Input.GetMouseButton(0))
                //{
                    Vector2 newVel = _steering.Calculate(AiTools.GetMousePosition());
                    UpdateTank(newVel);
                //}
            }
        }

        void UpdateTank(Vector2 _steeringForce)
        {
            UpdateEntity(_steeringForce);
        }

        private void OnDrawGizmos()
        {
            Vector2 vec2 = Velocity.normalized * AlertDistance;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AlertDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + vec2.x, transform.position.y + vec2.y, transform.position.z));// Velocity.normalized * 10);
        }

        [ContextMenu("Test")]
        void Test()
        {
            //_steering.AvoidObstacles();
        }
    }
}
