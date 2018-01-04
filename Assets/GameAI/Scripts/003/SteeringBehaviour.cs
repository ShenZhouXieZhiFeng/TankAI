using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

        //徘徊参数
        Vector2 wandarTarget;
        const float wandarRadius = 2.0f;
        const float wandarDistance = 3.0f;
        const float wandarJitter = 3.0f;//抖动
        #endregion

        #region func

        public SteeringBehaviour(MoveEntity _ent)
        {
            m_entity = _ent;
            m_transform = m_entity.transform;
            wandarTarget = m_transform.position;
        }

        public Vector2 Calculate()
        {
            return Vector2.one;
        }

        /// <summary>
        /// 追逐
        /// </summary>
        /// <returns></returns>
        public Vector2 Pursute(MoveEntity _target)
        {
            Vector2 toEvader = _target.Position - m_entity.Position;
            //计算与被追逐者朝向的夹角
            float relativeHeading = Vector2.Dot(_target.Heading, m_entity.Heading);
            //如果在被追逐者在前方20度以内
            if (Vector2.Dot(toEvader, m_entity.Heading) > 0 && relativeHeading < -0.95)
            {
                return Seek(_target.Position);
            }
            //如果不在前方20度以内,预测方向
            //预测的时间与距离成正比，与速度成反比
            float lookAheadTime = toEvader.magnitude / (m_entity.MaxVelocity + _target.Speed);
            lookAheadTime += TurnAroundTime(_target.Position);

            return Seek(_target.Position + _target.Velocity * lookAheadTime);
        }

        /// <summary>
        /// 逃避
        /// </summary>
        /// <returns></returns>
        public Vector2 Evade(MoveEntity _target)
        {
            Vector2 toTarget = _target.Position - m_entity.Position;
            //时间与距离成正比，与速度成反比
            float lookAheadTime = toTarget.magnitude / (m_entity.MaxVelocity * _target.Speed);
            lookAheadTime += TurnAroundTime(_target.Position);

            return Flee(_target.Position + _target.Velocity * lookAheadTime);
        }

        /// <summary>
        /// 徘徊
        /// </summary>
        /// <returns></returns>
        public Vector2 Wander()
        {
            wandarTarget += new Vector2(AiTools.RandomClamped() * wandarJitter, AiTools.RandomClamped() * wandarJitter);
            wandarTarget.Normalize();
            wandarTarget *= wandarRadius;
            Vector2 targetPos = wandarTarget + new Vector2(wandarDistance, 0);
            return targetPos - m_entity.Position;
        }

        /// <summary>
        /// 靠近
        /// </summary>
        /// <param name="_targetPos"></param>
        /// <returns></returns>
        Vector2 Seek(Vector2 _targetPos)
        {
            Vector2 desirdVelocity = (_targetPos - m_entity.Position).normalized * m_entity.MaxVelocity;
            return (desirdVelocity - m_entity.Velocity);
        }

        /// <summary>
        /// 远离
        /// </summary>
        /// <param name="_targetPos"></param>
        /// <returns></returns>
        Vector2 Flee(Vector2 _targetPos)
        {
            //进入警惕距离则远离
            float distance = Vector2.Distance(m_entity.Position, _targetPos);
            if (distance > m_entity.AlertDistance)
            {
                return Vector2.zero;
            }
            Vector2 desiredVelocity = (m_entity.Position - _targetPos).normalized * m_entity.MaxVelocity;
            return (desiredVelocity - m_entity.Velocity);
        }

        /// <summary>
        /// 到达
        /// </summary>
        /// <param name="_targetPos"></param>
        /// <returns></returns>
        Vector2 Arrive(Vector2 _targetPos, Deceleration _decel)
        {
            Vector2 toTarget = _targetPos - m_entity.Position;
            float distance = toTarget.magnitude;
            if (distance > 0)
            {
                const float decelTweaker = 0.3f;

                float speed = distance / ((float)_decel * decelTweaker);

                speed = Mathf.Min(speed, m_entity.MaxVelocity);

                Vector2 desiredVolecity = toTarget * speed / distance;

                return (desiredVolecity - m_entity.Velocity);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// 计算朝向目标旋转的时间,防止载体出现180度直接旋转的情况
        /// </summary>
        /// <param name="_target"></param>
        /// <returns></returns>
        float TurnAroundTime(Vector2 _targetPos)
        {
            Vector2 toTarget = (_targetPos - m_entity.Position).normalized;
            float dot = Vector2.Dot(m_entity.Heading, toTarget);
            //旋转的比率时间，与载体的最大旋转有关
            const float coeffcient = 0.5f;
            //如果目标直接在前方，dot = 1
            //如果目标在后方，dot = -1
            return (dot - 1) * -coeffcient;
        }

        #endregion

    }
}
