using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class FadeInOut
    {
        [HideInInspector]
        public bool isBlack = false;//��͸��״̬
        [HideInInspector]
        public float fadeSpeed = 1;//͸���ȱ仯����
        public RawImage rawImage;
        public RectTransform rectTransform;

        public void Start()
        {
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);//ʹ��������
            rawImage.color = Color.clear;
        }

        public void Update()
        {
            if (isBlack == false)
            {
                rawImage.color = Color.Lerp(rawImage.color, Color.clear, Time.deltaTime * fadeSpeed * 0.5f);//����
                //֮������ôд��Ҫ����ΪLerp������ԭ�򣬾��������Կ���ƪ����
                //��Unity��Lerp���÷���https://blog.csdn.net/MonoBehaviour/article/details/79085547
                if (rawImage.color.a < 0.1f)
                {
                    rawImage.color = Color.clear;
                }
            }
            else if (isBlack)
            {
                rawImage.color = Color.Lerp(rawImage.color, Color.black, Time.deltaTime * fadeSpeed);//����
                if (rawImage.color.a > 0.9f)
                {
                    rawImage.color = Color.black;
                }
            }
        }

        //�л�״̬
        public void BackGroundControl(bool b)
        {
            if (b == true)
                isBlack = true;
            else
                isBlack = false;
        }
    }
}

