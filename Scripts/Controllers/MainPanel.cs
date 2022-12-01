using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using QFramework;

namespace PlatformShoot
{
    public class MainPanel : PlatformShootGameController
    {
        private TextMeshProUGUI mScoreText;

        // Start is called before the first frame update
        void Start()
        {
            mScoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            transform.Find("SettingButton").GetComponent<Button>().onClick.AddListener(OnOpenSetting);
            this.GetModel<IGameModel>().Score.RegisterWithInitValue(OnScoreChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetModel<IGameModel>().Score.Value = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnScoreChanged(int score)
        {
            mScoreText.text = "Score: " + score.ToString();
        }

        private void OnOpenSetting()
        {
            ResHelper.AsyncLoad<GameObject>("Prefabs/SettingPanel", o =>
            {
                o.transform.SetParent(GameObject.Find("Canvas").transform);
                (o.transform as RectTransform).anchoredPosition = Vector2.zero;
            });

        }
    }
}
