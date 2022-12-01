using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ResPool<T> where T: UnityEngine.Object
    {
        private Dictionary<string, T> mResDic = new Dictionary<string, T>();
        public void Get(string key, Action<T> callBack)
        {
            if (mResDic.TryGetValue(key, out T data))
            {
                callBack(data);
                return;
            }
            ResHelper.AsyncLoad<T>(key, o =>
            {
                callBack(o);
                mResDic.Add(key, o);
            });
        }
        public void Clear() => mResDic.Clear();
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

