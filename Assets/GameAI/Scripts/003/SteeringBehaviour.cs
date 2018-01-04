using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class SteeringBehaviour
    {
        private enum behaviour_type
        {
            none                = 0x00000,
            seek                = 0x00002,
            flee                = 0x00004,
            arrive              = 0x00008,
            wander              = 0x00010,
            cohesion            = 0x00020,
            separation          = 0x00040,
            allignment          = 0x00080,
            obstacle_avoidance  = 0x00100,
            wall_avoidance      = 0x00200,
            follow_path         = 0x00400,
            pursuit             = 0x00800,
            evade               = 0x01000,
            interpose           = 0x02000,
            hide                = 0x04000,
            flock               = 0x08000,
            offset_persuit      = 0x10000
        };

        #region members

        MoveEntity m_entity;
        Transform m_transform;

        #endregion

        #region func

        public SteeringBehaviour(MoveEntity _ent)
        {
            m_entity = _ent;
            m_transform = m_entity.transform;
        }

        public Vector2 Calculate()
        {
            return Vector2.one;
        }

        public Vector2 Seek(Vector3 _targetPos)
        {
            Vector2 desirdVelocity = (_targetPos - m_transform.position).normalized * m_entity.MaxVelocity;
            return (desirdVelocity - m_entity.Velocity);
        }
        #endregion

    }
}
