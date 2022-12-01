using PlatformShoot;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public interface IFadeSceneSystem : ISystem
    {
        void Start();
        void Update();
        void BackGroundControl(bool b);
    }

    public class FadeSceneSystem : AbstractSystem, IFadeSceneSystem
    {
        private FadeInOut mFade;
        private GameObject mRawImage;
        public void BackGroundControl(bool b)
        {
            mFade.BackGroundControl(b);
        }

        public void Start()
        {
            mFade.Start();
        }

        public void Update()
        {
            mFade.Update();
        }

        protected override void OnInit()
        {
            mFade = new FadeInOut();
            if (mRawImage == null)
            {
                this.GetSystem<IObjectPoolSystem>().Get("Prefabs/RawImage", o =>
                {
                    mRawImage = o;
                });
            }
            mFade.rawImage = mRawImage.GetComponent<RawImage>();
            mFade.rectTransform = mRawImage.GetComponent<RectTransform>();
        }
    }
}


