using PlatformShoot;
using QFramework;
using System.Linq;
using UnityEngine;

namespace FPSVR
{
    public class VRPlayer : MonoBehaviour, IController
    {
        private Rigidbody mRig;
        private CapsuleCollider mCapColl;
        private LayerMask mGroundLayer;
        private float mInputX, mInputY;
        private float mJumpForce;
        private bool mJumpInput, isJumping;

        private float mAccDelta = 0.6f;
        private float mDecDelta = 0.9f;
        private float mGroundMoveSpeed = 5f;
        private Vector3 lastPos;

        private Collider[] mGround = {};

        private IObjectPoolSystem objectPool;
        private IAudioMgrSystem audioMgr;
        private void Start()
        {
            mRig = GetComponent<Rigidbody>();
            mCapColl = GetComponent<CapsuleCollider>();
            mGroundLayer = LayerMask.GetMask("Ground");
            mJumpForce = 12;
            mJumpInput = false;
            lastPos = transform.position;
            objectPool = this.GetSystem<IObjectPoolSystem>();
            audioMgr = this.GetSystem<IAudioMgrSystem>();
            this.RegisterEvent<MoveEvent>(e =>
            {
                mInputX = e.inputX;
                mInputY = e.inputY;
            });
            this.RegisterEvent<JumpEvent>(e =>
            {
                if (mGround.Count() > 0)
                {
                    audioMgr.PlaySound("ÌøÔ¾");
                    mJumpInput = true;
                    isJumping = true;
                    lastPos = transform.position;
                }
            });
        }

        private void Update()
        {
            mGround = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, mGroundLayer);
            if (mGround.Count() > 0 && isJumping)
            {
                audioMgr.PlaySound("ÂäµØ");
                isJumping = false;
            }
        }

        private void FixedUpdate()
        {
            if (mJumpInput)
            {
                mRig.velocity = new Vector3(mRig.velocity.x, mJumpForce, mRig.velocity.z);
                mJumpInput = false;
            }
            if (mInputX != 0 || mInputY != 0)
            {
                mRig.velocity = new Vector3(
                    Mathf.Clamp(mRig.velocity.x + mInputX * mAccDelta, -mGroundMoveSpeed, mGroundMoveSpeed), 
                    mRig.velocity.y,
                    Mathf.Clamp(mRig.velocity.z + mInputY * mAccDelta, -mGroundMoveSpeed, mGroundMoveSpeed)
                    );
            }
            else
            {
                mRig.velocity = new Vector3(
                    Mathf.MoveTowards(mRig.velocity.x, 0, mDecDelta), 
                    mRig.velocity.y,
                    Mathf.MoveTowards(mRig.velocity.z, 0, mDecDelta)
                    );
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Bound"))
            {
                transform.position = lastPos;
                mRig.velocity = Vector3.zero;
            }
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return FPSVR.Interface;
        }
    }
}

