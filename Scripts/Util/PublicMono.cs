using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformShoot
{
    public class PublicMono : MonoSingle<PublicMono>
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        public event Action OnFixedUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
    }
}

