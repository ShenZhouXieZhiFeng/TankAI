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
                if (Input.GetMouseButton(0))
                {
                    //Vector2 newVel = _steering.Seek(AiTools.GetMousePosition());
                    Vector2 newVel = _steering.Arrive(AiTools.GetMousePosition(), Deceleration.fast);
                    UpdateTank(newVel);
                }
            }
        }

        void UpdateTank(Vector2 _steeringForce)
        {
            Vector2 steeringForce = _steeringForce;
            Vector2 acceleration = steeringForce / Mass;
            acceleration = Vector2.ClampMagnitude(acceleration, MaxForce);
            AcceleratedVeloCity = acceleration;

            Velocity += acceleration * Time.deltaTime;
            //Velocity = Vector2.ClampMagnitude(Velocity, MaxVelocity);

            //transform.position += (Vector3)(acceleration * Time.deltaTime);

            UpdateEntity();
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
