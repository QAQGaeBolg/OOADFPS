using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class MonoSingle<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    var o = new GameObject(typeof(T).Name);
                    mInstance = o.AddComponent<T>();
                    GameObject.DontDestroyOnLoad(o);
                }
                return mInstance;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}


