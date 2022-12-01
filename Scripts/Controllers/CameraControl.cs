using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlatformShoot
{
    public class CameraControl : MonoBehaviour
    {
        private Transform mTarget;
        
        // Start is called before the first frame update
        void Start()
        {
            mTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void LateUpdate()
        {
            transform.localPosition = new Vector3(mTarget.position.x, mTarget.position.y, transform.position.z);
        }
    }
}
