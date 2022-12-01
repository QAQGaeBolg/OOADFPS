using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace PlatformShoot
{
    public class Bullet : MonoBehaviour, IController
    {
        public float mBulletSpeed;
        public int bulletDir;

        private LayerMask mLayerMask;

        private Timer mTimer;

        private void Awake()
        {
            mBulletSpeed = 12;
            mLayerMask = LayerMask.GetMask("Ground", "Trigger");
        }
        private void OnEnable()
        {
            mTimer = this.GetSystem<ITimerSystem>().AddTimer(3f, () => this.GetSystem<IObjectPoolSystem>().Recovery(gameObject));
        }
        private void OnDisable()
        {
            mTimer.Stop();
        }
        void Update()
        {
            transform.Translate(bulletDir * mBulletSpeed * Time.deltaTime, 0, 0);
        }

        private void FixedUpdate()
        {
            var coll = Physics2D.OverlapBox(transform.position, transform.localScale, 0, mLayerMask);
            if (coll)
            {
                if (coll.CompareTag("Trigger"))
                {
                    GameObject.Destroy(coll.gameObject);
                    this.SendCommand<ShowPassDoorCommand>();
                    this.GetSystem<IAudioMgrSystem>().PlaySound("Éä»÷»ú¹Ø");
                }
                else
                {
                    this.GetSystem<IAudioMgrSystem>().PlaySound("Éä»÷Ç½±Ú");
                }
                this.GetSystem<IObjectPoolSystem>().Recovery(gameObject);
            }
        }

        public void InitDir(int dir)
        {
            bulletDir = dir;
        }

        public IArchitecture GetArchitecture()
        {
            return PlatformShootGame.Interface;
        }
    }
}
