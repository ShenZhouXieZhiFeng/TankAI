using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameAI
{
    public class SteeringBehaviour
    {
        #region members

        MoveEntity m_entity;
        Transform m_transform;

        //徘徊参数
        Vector2 wandarTarget;
        const float wandarRadius = 2.0f;
        const float wandarDistance = 1.0f;
        const float wandarJitter = 3.0f;//抖动

        //躲避障碍参数
        List<TankSensor> sensers;
        #endregion

        #region func

        public SteeringBehaviour(MoveEntity _ent)
        {
            m_entity = _ent;
            m_transform = m_entity.transform;
            wandarTarget = m_transform.position;
        }

        public Vector2 Calculate(Vector2 _targetPos)
        {
            Vector2 newAcce = Vector2.zero;

            newAcce += Arrive(_targetPos, Deceleration.fast);

            return newAcce;
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

        //public Vector2 AvoidObstacles()
        //{
        //    if (sensers == null)
        //    {
        //        sensers = new List<TankSensor>(m_transform.GetComponentsInChildren<TankSensor>());
        //    }
        //    if (sensers == null)
        //        return Vector2.zero;

        //    List<float> senserLens = new List<float>();
        //    foreach (var senser in sensers)
        //    {
        //        senserLens.Add(senser.CurrentLength);
        //    }
        //    if (senserLens[0] < senserLens[2])
        //    {
        //        return m_transform.right * Time.deltaTime;
        //    }
        //    else if (senserLens[0] > senserLens[2])
        //    {
        //        return -m_transform.right * Time.deltaTime;
        //    }
        //    return Vector2.zero;
        //}

        /// <summary>
        /// 徘徊
        /// </summary>
        /// <returns></returns>
        Vector2 Wander()
        {
            wandarTarget += new Vector2(AiTools.RandomClamped() * wandarJitter, AiTools.RandomClamped() * wandarJitter);
            wandarTarget = wandarTarget.normalized * wandarRadius;
            Vector2 targetLocal = wandarTarget + Vector2.right * wandarDistance;
            Vector2 targetWorld = m_transform.TransformPoint(targetLocal);
            return (targetWorld - m_entity.Position);
        }

        /// <summary>
        /// 靠近
        /// </summary>
        /// <param name="_targetPos"></param>
        /// <returns></returns>
        Vector2 Seek(Vector2 _targetPos)
        {
            Vector2 desirdVelocity = (_targetPos - m_entity.Position).normalized * m_entity.MaxVelocity;
            //乘上Time.deltaTime使得加速度的改变量减小，可以有一种位移偏移的效果
            return (desirdVelocity - m_entity.Velocity) * Time.deltaTime;
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
            return (desiredVelocity - m_entity.Velocity) * Time.deltaTime;
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
            if (distance > 0.1f)
            {
                float decelTweaker = 0.3f;
                float speed = distance / ((int)_decel * decelTweaker);
                speed = Math.Min(speed, m_entity.MaxVelocity);
                Vector2 desiredVolecity = toTarget * speed / speed;
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
            float dot = Vector2.Dot(m_entity.Heading.normalized, toTarget);
            //旋转的比率时间，与载体的最大旋转有关
            float coeffcient = 0.5f;
            //如果目标直接在前方，dot = 1
            //如果目标在后方，dot = -1
            return (dot - 1) * -coeffcient;
        }

        #endregion

    }
}
