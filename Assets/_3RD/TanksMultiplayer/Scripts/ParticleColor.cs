/*  This file is part of the "Tanks Multiplayer" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;

namespace TanksMP
{
    /// <summary>
    /// Modifies the starting color of a particle system to the color passed in.
    /// This is used on the player death particles to match the player's team color.
    /// </summary>
    public class ParticleColor : MonoBehaviour
    {
        /// <summary>
        /// Array for particle systems that should be colored.
        /// </summary>
        public ParticleSystem[] particles;

        /// <summary>
        /// Iterates over all particles and assigns the color passed in,
        /// but ignoring the alpha value of the new color.
        /// </summary>
        public void SetColor(Color color)
        {
            for(int i = 0; i < particles.Length; i++)
            {
                #if UNITY_5_5_OR_NEWER
                    color.a = particles[i].main.startColor.color.a;
                    ParticleSystem.MainModule mainMod = particles[i].main;
                    mainMod.startColor = color;
                #else
                    color.a = particles[i].startColor.a;
                    particles[i].startColor = color;
                #endif
            }
        }
    }
}

