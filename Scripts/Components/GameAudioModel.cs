using UnityEngine;

namespace QFramework
{
    public interface IGameAudioModel: IModel
    {
        BindableProperty<float> BgmVolume { get; }
        BindableProperty<float> SoundVolume { get; }
    }

    public class GameAudioModel: AbstractModel, IGameAudioModel
    {
        public BindableProperty<float> BgmVolume { get; } = new BindableProperty<float>(0.5f);
        public BindableProperty<float> SoundVolume { get; } = new BindableProperty<float>(0.5f);
        protected override void OnInit()
        {
            BgmVolume.Value = PlayerPrefs.GetFloat(nameof(BgmVolume), 0.5f);
            SoundVolume.Value = PlayerPrefs.GetFloat(nameof(SoundVolume), 1f);

            BgmVolume.Register(value => PlayerPrefs.SetFloat(nameof(BgmVolume), value));
            SoundVolume.Register(value => PlayerPrefs.SetFloat(nameof(SoundVolume), value));
        }
    }
}
