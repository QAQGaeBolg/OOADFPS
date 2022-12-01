using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class FadeInOut
    {
        [HideInInspector]
        public bool isBlack = false;//不透明状态
        [HideInInspector]
        public float fadeSpeed = 1;//透明度变化速率
        public RawImage rawImage;
        public RectTransform rectTransform;

        public void Start()
        {
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);//使背景满屏
            rawImage.color = Color.clear;
        }

        public void Update()
        {
            if (isBlack == false)
            {
                rawImage.color = Color.Lerp(rawImage.color, Color.clear, Time.deltaTime * fadeSpeed * 0.5f);//渐亮
                //之所以这么写主要是因为Lerp函数的原因，具体详解可以看这篇文章
                //【Unity中Lerp的用法】https://blog.csdn.net/MonoBehaviour/article/details/79085547
                if (rawImage.color.a < 0.1f)
                {
                    rawImage.color = Color.clear;
                }
            }
            else if (isBlack)
            {
                rawImage.color = Color.Lerp(rawImage.color, Color.black, Time.deltaTime * fadeSpeed);//渐暗
                if (rawImage.color.a > 0.9f)
                {
                    rawImage.color = Color.black;
                }
            }
        }

        //切换状态
        public void BackGroundControl(bool b)
        {
            if (b == true)
                isBlack = true;
            else
                isBlack = false;
        }
    }
}

