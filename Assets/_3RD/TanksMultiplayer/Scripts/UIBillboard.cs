/*  This file is part of the "Tanks Multiplayer" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;

namespace TanksMP
{
    /// <summary>
    /// Orientates the gameobject this script is attached to to always face the camera.
    /// </summary>
    public class UIBillboard : MonoBehaviour
    {
        //optimize GetComponent calls:
        //cache reference to camera transform
        private Transform camTrans;
        
        //cache reference to this transform
        private Transform trans;


        //initialize variables
        void Awake()
        {
            camTrans = Camera.main.transform;
            trans = transform;
        }


        //always face the camera every frame
        void Update()
        {
            transform.LookAt(trans.position + camTrans.rotation * Vector3.forward,
                            camTrans.rotation * Vector3.up);
        }
    }
}