using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace PlatformShoot
{
    public class StartPanel : PlatformShootGameController
    {
        private IAudioMgrSystem mAudioMgr;
        private void Awake()
        {
            transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartButton);
            transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnExitButton);
        }
        private void Start()
        {
            mAudioMgr = this.GetSystem<IAudioMgrSystem>();
            mAudioMgr.PlayBgm("bgm6");
        }

        private void OnStartButton()
        {
            this.SendCommand(new NextLevelCommand("SampleLevel"));
            mAudioMgr.StopBgm(false);
        }

        private void OnExitButton()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}

