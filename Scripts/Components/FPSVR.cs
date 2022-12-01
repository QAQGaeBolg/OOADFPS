using PlatformShoot;
using QFramework;
using UnityEngine;

namespace FPSVR
{
    public class FPSVR : Architecture<FPSVR>
    {
        protected override void Init()
        {
            RegisterModel<IGameAudioModel>(new GameAudioModel());
            RegisterSystem<ITimerSystem>(new TimerSystem());
            RegisterSystem<ICameraSystem>(new CameraSystem());
            RegisterSystem<IAudioMgrSystem>(new AudioMgrSystem());
            RegisterSystem<IObjectPoolSystem>(new ObjectPoolSystem());
            RegisterSystem<IPlayerVRInputSystem>(new PlayerVRInputSystem());
            RegisterSystem<IFadeSceneSystem>(new FadeSceneSystem());
        }
    }
    public class FPSVRController : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture()
        {
            return FPSVR.Interface;
        }
    }
}
