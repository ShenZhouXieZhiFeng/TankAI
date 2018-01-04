using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class AiTools
    {

        public static Vector2 GetMousePosition()
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(pos.x, pos.y);
        }

        public static float RandomClamped()
        {
            return Random.Range(0f, 1f);
        }
        
    }
}
