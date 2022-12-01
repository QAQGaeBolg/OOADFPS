using PlatformShoot;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace QFramework
{
    public interface IAudioMgrSystem : ISystem
    {
        void PlayBgm(string name);
        void StopBgm(bool isPause);
        void PlaySound(string name);
        AudioSource GetSound(string name);

        void RecoverySound(AudioSource source);
        void Clear();
    }
    public class AudioMgrSystem : AbstractSystem, IAudioMgrSystem
    {
        private AudioSource mBGM;
        private AudioSource tempSource;
        private FadeNum mFade;
        private ResPool<AudioClip> mClipPool;
        private ComponentPool<AudioSource> mSourcePool;
        private IGameAudioModel mAudioModel;
        protected override void OnInit()
        {
            mSourcePool = new ComponentPool<AudioSource>("GameSound");
            mClipPool = new ResPool<AudioClip>();
            mAudioModel = this.GetModel<IGameAudioModel>();
            mFade = new FadeNum();
            mFade.SetMinMax(0, mAudioModel.BgmVolume.Value);
            mAudioModel.BgmVolume.RegisterWithInitValue(OnBGMVolumeChanged);
            mAudioModel.SoundVolume.RegisterWithInitValue(OnSoundVolumeChanged);
            PublicMono.Instance.OnUpdate += UpdateVolume;
        }
        private void Update()
        {
            if (!mFade.isEnabled)
            {
                return;
            }
            mFade.Update(Time.deltaTime);
            mBGM.volume = mFade.CurrentValue;
        }
        void IAudioMgrSystem.PlaySound(string name)
        {
            InitSource();
            mClipPool.Get("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.loop = false;
                tempSource.Play();
            });
        }
        AudioSource IAudioMgrSystem.GetSound(string name)
        {
            InitSource();
            mClipPool.Get("Audio/Sound/" + name, clip =>
            {
                tempSource.clip = clip;
                tempSource.loop = true;
            });
            return tempSource;
        }
        void IAudioMgrSystem.RecoverySound(AudioSource source)
        {
            mSourcePool.Push(source, source.Stop);
        }
        void IAudioMgrSystem.PlayBgm(string name)
        {
            mClipPool.Get("Audio/BGM/" + name, PlayBgm);
        }
        void IAudioMgrSystem.StopBgm(bool isPause)
        {
            if (mBGM == null || !mBGM.isPlaying)
            {
                return;
            }
            mFade.SetState(FadeState.FadeOut, () =>
            {
                if (isPause)
                {
                    mBGM.Pause();
                }
                else
                {
                    mBGM.Stop();
                }
            });
        }
        void IAudioMgrSystem.Clear()
        {
            mClipPool.Clear();
        }
        private void PlayBgm(AudioClip clip)
        {
            if (mBGM == null)
            {
                var o = new GameObject("GameBGM");
                GameObject.DontDestroyOnLoad(o);
                mBGM = o.AddComponent<AudioSource>();
                mBGM.loop = true;
                mBGM.volume = 0;
            }
            mBGM.clip = clip;
            if (!mBGM.isPlaying)
            {
                PlayBgm();
            }
            else
            {
                mFade.SetState(FadeState.FadeOut, PlayBgm);
            }
        }
        void PlayBgm()
        {
            mFade.SetState(FadeState.FadeIn);
            mBGM.Play();
        }
        private void OnBGMVolumeChanged(float v)
        {
            if (mBGM == null)
            {
                return;
            }
            mFade.SetMinMax(0, v);
            mBGM.volume = v;
        }
        
        private void UpdateVolume()
        {
            if (!mFade.isEnabled)
            {
                return;
            }
            mBGM.volume = mFade.Update(Time.deltaTime);
        }
        public void InitSource()
        {
            mSourcePool.AutoPush(source => !source.isPlaying);
            mSourcePool.Get(out tempSource);
            tempSource.volume = mAudioModel.SoundVolume.Value;
        }
        private void OnSoundVolumeChanged(float v)
        {
            mSourcePool.SetAllEnabledComponents(source => source.volume = v);
        }
    }
}

