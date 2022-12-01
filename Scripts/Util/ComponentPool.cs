using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ComponentPool<T> where T: Behaviour
    {
        private GameObject mRoot; // 挂载组件
        private string mRootName;
        private List<T> mOpenList = new List<T>(); // 储存已激活组件
        private Queue<T> mCloseList = new Queue<T>(); // 储存未使用组件
        public ComponentPool(string rootObjName)
        {
            mRootName = rootObjName;
        }
        public void SetAllEnabledComponents(Action<T> callBack)
        {
            foreach (T component in mOpenList)
            {
                callBack(component);
            }
        }
        // 获取可使用组件
        public void Get(out T component)
        {
            if (mCloseList.Count > 0)
            {
                component = mCloseList.Dequeue();
                component.enabled = true;
            }
            else
            {
                if (mRoot == null)
                {
                    mRoot = new GameObject(mRootName);
                    GameObject.DontDestroyOnLoad(mRoot);
                }
                component = mRoot.AddComponent<T>();
            }
            mOpenList.Add(component);
        }
        // 自动回收组件
        public void AutoPush(Func<T, bool> condition)
        {
            for (int i = mOpenList.Count - 1; i >= 0; i--)
            {
                if (condition(mOpenList[i]))
                {
                    mOpenList[i].enabled = false;
                    mCloseList.Enqueue(mOpenList[i]);
                    mOpenList.RemoveAt(i);
                }
            }
        }
        // 回收单个组件
        public void Push(T component, Action callBack = null)
        {
            if (mOpenList.Contains(component))
            {
                callBack?.Invoke();
                mOpenList.Remove(component);
                component.enabled = false;
                mCloseList.Enqueue(component);
            }
        }
    }
}

