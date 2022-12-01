
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPSVR
{
    public class MainMenuManager : MonoBehaviour, IController
    {
        public FadeInOut mFadeInOut;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void NewGame()
        {
            this.GetSystem<IFadeSceneSystem>().BackGroundControl(true);
            SceneManager.LoadScene("Level1");
            this.GetSystem<IFadeSceneSystem>().BackGroundControl(false);
        }

        public void Quit()
        {
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
            #else
		            Application.Quit();
            #endif
        }

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return FPSVR.Interface;
        }
    }
}

