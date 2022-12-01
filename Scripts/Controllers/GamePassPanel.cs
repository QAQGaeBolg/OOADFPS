using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlatformShoot
{
    public class GamePassPanel : PlatformShootGameController
    {
        private IAudioMgrSystem mAudioMgr;
        // Start is called before the first frame update
        void Start()
        {
            transform.Find("ResetGameButton").GetComponent<Button>().onClick.AddListener(OnRestart);
            mAudioMgr = this.GetSystem<IAudioMgrSystem>();
            mAudioMgr.PlayBgm("bgm4");
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnRestart()
        {
            SceneManager.LoadScene("SampleLevel");
            mAudioMgr.StopBgm(false);
        }
    }
}


