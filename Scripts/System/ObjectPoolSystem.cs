using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IObjectPoolSystem : ISystem
    {
        GameObject Get(string name);
        void Get(string name, Action<GameObject> callBack = null);
        void Recovery(GameObject obj);
        void Dispose();
    }

    public class PoolData
    {
        private Queue<GameObject> mActivatableObject = new Queue<GameObject>();
        public bool CanGet => mActivatableObject.Count > 0;
        private Transform mFatherObj;
        public PoolData(GameObject obj, Transform root)
        {
            mFatherObj = new GameObject(obj.name).transform;
            mFatherObj.SetParent(root);
            Push(obj);
        }
        public GameObject Get()
        {
            GameObject obj = mActivatableObject.Dequeue();
            obj.SetActive(true);
            obj.transform.SetParent(null);
            return obj;
        }
        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(mFatherObj.transform);
            mActivatableObject.Enqueue(obj);
        }
    }

    public class ObjectPoolSystem : AbstractSystem, IObjectPoolSystem
    {
        private Dictionary<string, PoolData> mPoolDic;
        private Transform mPoolRoot;
        GameObject IObjectPoolSystem.Get(string name)
        {
            return mPoolDic.TryGetValue(name, out PoolData data) && data.CanGet ? data.Get() : new GameObject(name);
        }

        void IObjectPoolSystem.Get(string name, Action<GameObject> callBack)
        {
            if (mPoolDic.TryGetValue(name, out PoolData data) && data.CanGet)
            {
                if (callBack == null)
                {
                    data.Get();
                }
                else
                {
                    callBack(data.Get());
                }
                return;
            }
            ResHelper.AsyncLoad<GameObject>(name, o =>
            {
                o.name = name;
                callBack?.Invoke(o);
            });

        }
        void IObjectPoolSystem.Recovery(GameObject obj)
        {
            if (mPoolDic.TryGetValue(obj.name, out var data))
            {
                data.Push(obj);
                return;
            }
            if (mPoolRoot == null)
            {
                mPoolRoot = new GameObject("PoolRoot").transform;
            }
            mPoolDic.Add(obj.name, new PoolData(obj, mPoolRoot));
        }
        void IObjectPoolSystem.Dispose()
        {
            mPoolDic.Clear();
            mPoolRoot = null;
        }
        protected override void OnInit()
        {
            mPoolDic = new Dictionary<string, PoolData>();
        }
    }
}

