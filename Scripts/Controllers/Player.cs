using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using QFramework;

namespace PlatformShoot
{
    public class Player : MonoBehaviour, IController
    {
        private Rigidbody2D mRig;
        private BoxCollider2D mBoxColl;
        private LayerMask mGroundLayer;

        private float mAccDelta = 0.6f;
        private float mDecDelta = 0.9f;
        private float mGroundMoveSpeed;
        private float mJumpForce;
        private bool mJumpInput;
        public int mFaceFir;
        private bool isJumping;
        private Vector3 lastPos;

        private bool mAttackImput;

        private bool mGround;

        private int mInputX, mInputY;

        private IObjectPoolSystem objectPool;
        private IAudioMgrSystem audioMgr;
        
        // Start is called before the first frame update
        void Start()
        {
            mRig = GetComponent<Rigidbody2D>();
            mBoxColl = GetComponentInChildren<BoxCollider2D>();
            mGroundLayer = LayerMask.GetMask("Ground");
            mGroundMoveSpeed = 5;
            mJumpForce = 12;
            mJumpInput = false;
            mFaceFir = 1;
            isJumping = false;
            lastPos = transform.position;
            this.GetSystem<ICameraSystem>().SetTarget(transform);
            objectPool = this.GetSystem<IObjectPoolSystem>();
            audioMgr = this.GetSystem<IAudioMgrSystem>();
            audioMgr.PlayBgm("bgm7");
            this.RegisterEvent<DirInputEvent>(e =>
            {
                mInputX = e.inputX;
                mInputY = e.inputY;
            });
            this.RegisterEvent<ShootInputEvent>(e =>
            {
                mAttackImput = e.isTrigger;
            });
            this.RegisterEvent<JumpInputEvent>(e =>
            {
                if (mGround)
                {
                    audioMgr.PlaySound("跳跃");
                    mJumpInput = true;
                    isJumping = true;
                    lastPos = transform.position;
                }
            });
        }

        private void Update()
        {
            if (mAttackImput)
            {
                mAttackImput = false;
                audioMgr.PlaySound("竖琴1");
                objectPool.Get("Prefabs/Bullet", o =>
                {
                    o.transform.localPosition = transform.position;
                    var bullet = o.GetComponent<Bullet>();
                    bullet.InitDir(mFaceFir);
                });
            }
            mGround = Physics2D.OverlapBox(transform.position + mBoxColl.size.y * Vector3.down * 0.5f, new Vector2(mBoxColl.size.x * 0.8f, 0.1f), 0, mGroundLayer);

            if (mGround && isJumping)
            {
                audioMgr.PlaySound("落地");
                isJumping = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + mBoxColl.size.y * Vector3.down * 0.5f, new Vector2(mBoxColl.size.x * 0.8f, 0.1f));
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (mJumpInput)
            {
                mRig.velocity = new Vector2(mRig.velocity.x, mJumpForce);
                mJumpInput = false;
            }
            if (mInputX != 0)
            {
                mRig.velocity = new Vector2(Mathf.Clamp(mRig.velocity.x + mInputX * mAccDelta, -mGroundMoveSpeed, mGroundMoveSpeed), mRig.velocity.y);
            } else
            {
                mRig.velocity = new Vector2(Mathf.MoveTowards(mRig.velocity.x, 0, mDecDelta), mRig.velocity.y);
            }
            Flip(mInputX);
        }

        public void Flip(float h)
        {
            if (h != 0 && h != mFaceFir)
            {
                mFaceFir = -mFaceFir;
                transform.Rotate(0, 180, 0);
            }
        }

        private void LateUpdate()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Reward"))
            {
                GameObject.Destroy(collision.gameObject);
                this.GetModel<IGameModel>().Score.Value++;
                audioMgr.PlaySound("金币");
            }
            if (collision.gameObject.CompareTag("Door"))
            {
                this.SendCommand(new NextLevelCommand("GamePassScene"));
                audioMgr.PlaySound("通关1");
            }
            if (collision.gameObject.CompareTag("Bound"))
            {
                transform.position = lastPos;
                mRig.velocity = Vector2.zero;
            }
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return PlatformShootGame.Interface;
        }
    }
}


