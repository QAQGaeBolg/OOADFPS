using PlatformShoot;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPSVR
{
    public class VRBullet : MonoBehaviour, IController
    {
        public float mBulletSpeed;
        public Vector3 bulletDir;

        private LayerMask mLayerMask;

        private Timer mTimer;

        private void Awake()
        {
            mBulletSpeed = 12;
            mLayerMask = LayerMask.GetMask("Ground", "Trigger", "Monster");
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
            transform.Translate(
                bulletDir.x * mBulletSpeed * Time.deltaTime, 
                bulletDir.y * mBulletSpeed * Time.deltaTime,
                bulletDir.z * mBulletSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            var colls = Physics.OverlapBox(transform.position, transform.localScale, Quaternion.identity, mLayerMask);
            for (int i = 0; i < colls.Count(); i++)
            {
                var coll = colls[i];
                if (coll.CompareTag("Trigger"))
                {
                    GameObject.Destroy(coll.gameObject);
                    this.SendCommand<ShowPassDoorCommand>();
                    this.GetSystem<IAudioMgrSystem>().PlaySound("Éä»÷»ú¹Ø");
                }
                else if (coll.CompareTag("Ground"))
                {
                    this.GetSystem<IAudioMgrSystem>().PlaySound("Éä»÷Ç½±Ú");
                }
                else if (coll.CompareTag("Monster"))
                {
                    this.SendCommand<ShowPassDoorCommand>();
                }
                this.GetSystem<IObjectPoolSystem>().Recovery(gameObject);
            }
        }

        public void InitDir(Vector3 dir)
        {
            bulletDir = dir;
        }

        public IArchitecture GetArchitecture()
        {
            return PlatformShootGame.Interface;
        }
    }
}

