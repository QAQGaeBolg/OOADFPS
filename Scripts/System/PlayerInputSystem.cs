using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformShoot
{
    public interface IPlayerInputSystem: ISystem
    {
        void Enable();
        void Disable();
    }

    public struct DirInputEvent
    {
        public int inputX, inputY;
    }
    public struct JumpInputEvent
    {

    }
    public struct ShootInputEvent
    {
        public bool isTrigger;
    }
    public class PlayerInputSystem : AbstractSystem, IPlayerInputSystem, GameControllers.IGamePlayActions
    {
        private GameControllers mControllers = new GameControllers();
        private DirInputEvent dirInputEvent;
        private ShootInputEvent shootInputEvent;
        private float sensititvity = 0.3f;
        protected override void OnInit()
        {
            mControllers.GamePlay.SetCallbacks(this);
            mControllers.GamePlay.Enable();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            this.SendEvent<JumpInputEvent>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 input = context.ReadValue<Vector2>();
                dirInputEvent.inputX = Mathf.Abs(input.x) < sensititvity ? 0 : input.x < 0 ? -1 : 1;
                dirInputEvent.inputY = Mathf.Abs(input.y) < sensititvity ? 0 : input.y < 0 ? -1 : 1;
                this.SendEvent(dirInputEvent);
            }
            else if (context.canceled)
            {
                dirInputEvent.inputX = 0;
                dirInputEvent.inputY = 0;
                this.SendEvent(dirInputEvent);
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            shootInputEvent.isTrigger = context.ReadValueAsButton();
            this.SendEvent(shootInputEvent);
        }

        public void Enable()
        {
            mControllers.GamePlay.Enable();
        }

        public void Disable()
        {
            mControllers.GamePlay.Disable();
        }
    }
}
