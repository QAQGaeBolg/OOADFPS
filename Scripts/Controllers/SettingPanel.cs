using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using QFramework;

namespace PlatformShoot
{
    public class SettingPanel : PlatformShootGameController
    {
        private IGameAudioModel gameAudio;
        private void Awake()
        {
            gameAudio = this.GetModel<IGameAudioModel>();
            transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseSelf);

            var bgm = transform.Find("BGMVolume").GetComponent<Slider>();
            bgm.value = gameAudio.BgmVolume.Value;
            bgm.onValueChanged.AddListener(OnBGMValueChanged);
            var sound = transform.Find("SoundVolume").GetComponent<Slider>();
            sound.value = gameAudio.SoundVolume.Value;
            sound.onValueChanged.AddListener(OnSoundValueChanged);
            
        }

        private void OnCloseSelf()
        {
            Destroy(gameObject);
        }

        private void OnBGMValueChanged(float value)
        {
            gameAudio.BgmVolume.Value = value;
        }

        private void OnSoundValueChanged(float value)
        {
            gameAudio.SoundVolume.Value = value;
        }
    }
}

