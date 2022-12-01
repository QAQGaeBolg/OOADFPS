using PlatformShoot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class Timer
    {
        private Action OnFinished;
        private float mFinishTime;
        private float mDelayTime;
        private bool mLoop;
        private bool mIsFinish;
        public bool IsFinish => mIsFinish;

        public void Start(Action onFinished, float delayTime, bool isLoop)
        {
            OnFinished = onFinished;
            mFinishTime = Time.time + delayTime;
            mDelayTime = delayTime;
            mLoop = isLoop;
            mIsFinish = false;
        }
        public void Stop() => mIsFinish = true;

        public void Update()
        {
            if (mIsFinish)
            {
                return;
            }
            if (Time.time < mFinishTime)
            {
                return;
            }
            if (!mLoop)
            {
                Stop();
            }
            else
            {
                mFinishTime = Time.time + mDelayTime;
            }
            OnFinished?.Invoke();
        }
    }
    public interface ITimerSystem : ISystem
    {
        Timer AddTimer(float delayTime, Action onFinished, bool isLoop = false);
    }
    public class TimerSystem : AbstractSystem, ITimerSystem
    {
        private List<Timer> mUpdateList = new List<Timer>();
        private Queue<Timer> mAvailableQueue = new Queue<Timer>();
        public Timer AddTimer(float delayTime, Action onFinished, bool isLoop)
        {
            var timer = mAvailableQueue.Count == 0 ? new Timer() : mAvailableQueue.Dequeue();
            timer.Start(onFinished, delayTime, isLoop);
            mUpdateList.Add(timer);
            return timer;
        }
        protected override void OnInit()
        {
            PublicMono.Instance.OnUpdate += Update;
        }
        private void Update()
        {
            if (mUpdateList.Count == 0)
            {
                return;
            }
            for (int i = mUpdateList.Count - 1; i >= 0; i--)
            {
                if (mUpdateList[i].IsFinish)
                {
                    mAvailableQueue.Enqueue(mUpdateList[i]);
                    mUpdateList.RemoveAt(i);
                    continue;
                }
                mUpdateList[i].Update();
            }
        }
    }
}

