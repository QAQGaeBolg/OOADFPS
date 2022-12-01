using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPSVR
{
    public interface IPlayerVRInputSystem : ISystem
    {
        void Enable();
        void Disable();
    }

    public struct MoveEvent
    {
        public int inputX, inputY;
    }
    public struct JumpEvent { }
    public struct MiniMapEvent { }
    public struct SettingEvent { }
    public struct MenuEvent { }
    public struct ShootEvent
    {
        public bool isLeft;
    }
    public struct GrapEvent
    {
        public bool isLeft;
    }
    public class PlayerVRInputSystem : AbstractSystem, IPlayerVRInputSystem, VRInput.IGamePlayActions
    {
        private VRInput mControllers = new VRInput();
        private MoveEvent moveEvent;
        private JumpEvent jumpEvent;
        private ShootEvent shootEvent;
        private GrapEvent grapEvent;
        private float sensititvity = 0.3f;
        protected override void OnInit()
        {
            mControllers.GamePlay.SetCallbacks(this);
            mControllers.GamePlay.Enable();
        }

        public void Enable()
        {
            mControllers.GamePlay.Enable();
        }

        public void Disable()
        {
            mControllers.GamePlay.Disable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 input = context.ReadValue<Vector2>();
                moveEvent.inputX = Mathf.Abs(input.x) < sensititvity ? 0 : input.x < 0 ? -1 : 1;
                moveEvent.inputY = Mathf.Abs(input.y) < sensititvity ? 0 : input.y < 0 ? -1 : 1;
                this.SendEvent(moveEvent);
            }
            else if (context.canceled)
            {
                moveEvent.inputX = 0;
                moveEvent.inputY = 0;
                this.SendEvent(moveEvent);
            }
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<JumpEvent>();
        }

        public void OnMiniMap(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<MiniMapEvent>();
        }

        public void OnSetting(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<SettingEvent>();
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<MenuEvent>();
        }

        public void OnLeftShoot(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            shootEvent.isLeft = true;
            this.SendEvent(shootEvent);
        }

        public void OnRightShoot(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            shootEvent.isLeft = false;
            this.SendEvent(shootEvent);
        }

        public void OnLeftGrap(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            grapEvent.isLeft = false;
            this.SendEvent(grapEvent);
        }

        public void OnRightGrap(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            grapEvent.isLeft = false;
            this.SendEvent(grapEvent);
        }
    }
}
