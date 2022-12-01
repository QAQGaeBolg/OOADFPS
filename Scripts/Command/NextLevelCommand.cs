using QFramework;
using UnityEngine.SceneManagement;

namespace PlatformShoot
{
    public class NextLevelCommand : AbstractCommand
    {
        private readonly string mSceneName;

        public NextLevelCommand(string name)
        {
            mSceneName = name;
        }
        protected override void OnExecute()
        {
            SceneManager.LoadScene(mSceneName);
        }
    }
}
