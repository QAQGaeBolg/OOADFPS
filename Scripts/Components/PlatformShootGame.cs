using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class PlatformShootGame : Architecture<PlatformShootGame>
    {
        protected override void Init()
        {
            RegisterModel<IGameModel>(new GameModels());
            RegisterModel<IGameAudioModel>(new GameAudioModel());
            RegisterSystem<ITimerSystem>(new TimerSystem());
            RegisterSystem<ICameraSystem>(new CameraSystem());
            RegisterSystem<IAudioMgrSystem>(new AudioMgrSystem());
            RegisterSystem<IObjectPoolSystem>(new ObjectPoolSystem());
            RegisterSystem<IPlayerInputSystem>(new PlayerInputSystem());
        }
    }
    public class PlatformShootGameController: MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture()
        {
            return PlatformShootGame.Interface;
        }
    }
}
