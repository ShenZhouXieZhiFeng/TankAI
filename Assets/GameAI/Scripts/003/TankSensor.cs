using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class TankSensor : MonoBehaviour
    {
        [Header("显示")]
        public bool Show = true;
        [Header("长度")]
        public float MaxLength = 3.0f;
        [Header("颜色")]
        public Color ShowColor = Color.blue;
        [Header("扫描层")]
        public LayerMask WallLayer;

        public float CurrentLength
        {
            get { return currentLength; }
            private set { currentLength = value; }
        }

        float currentLength = 0f;
        Vector2 Forward;

        void Start()
        {
            CurrentLength = MaxLength;
        }

        private void Update()
        {
            RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, Forward, MaxLength, WallLayer);
            if (hitinfo)
            {
                CurrentLength = Vector2.Distance(transform.position, hitinfo.point);
            }
            else
            {
                CurrentLength = MaxLength;
            }
        }

        private void OnDrawGizmos()
        {
            if (Show)
            {
                Forward = transform.up;
                Gizmos.color = ShowColor;
                Vector2 to = (Vector2)transform.position + Forward.normalized * CurrentLength;
                Gizmos.DrawLine(transform.position, to);
            }
        }

    }
}
