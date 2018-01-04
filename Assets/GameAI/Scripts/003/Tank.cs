using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class Tank : MoveEntity
    {
        GameWorld _gameWorld;
        SteeringBehaviour _steering;

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
                    UpdateTank(_steering.Seek(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                }
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
            Vector2 vec2 = Velocity.normalized * 10;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + vec2.x, transform.position.y + vec2.y, transform.position.z));// Velocity.normalized * 10);

            Vector3 up = transform.up * 5;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + up.x, transform.position.x + up.y, transform.position.x + up.z));
        }
    }
}
