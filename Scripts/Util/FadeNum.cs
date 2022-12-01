using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public enum FadeState
    {
        Close,
        FadeIn,
        FadeOut,
    }
    public class FadeNum
    {
        private FadeState mFadeState = FadeState.Close;
        public bool isEnabled => mFadeState != FadeState.Close;
        // 调用委托的时候避免提前结束
        private bool mInit = false;
        // 淡入结束后做的事情
        private Action mOnFinish;
        private float mCurrentValue;
        public float CurrentValue => mCurrentValue;
        private float mMin = 0, mMax = 1;
        public void SetMinMax(float min, float max)
        {
            mMin = min;
            mMax = max;
        }
        public void SetState(FadeState state, Action action = null)
        {
            mOnFinish = action;
            mFadeState = state;
            mInit = false;
        }
        private void OnFinish(float value)
        {
            mOnFinish?.Invoke();
            mCurrentValue = value;
            if (!mInit)
            {
                return;
            }
            mFadeState = FadeState.Close;
        }
        public float Update(float step)
        {
            switch (mFadeState)
            {
                case FadeState.FadeIn:
                    if (!mInit)
                    {
                        mCurrentValue = mMin;
                        mInit = true;
                    }
                    if (mCurrentValue < mMax)
                    {
                        mCurrentValue += step;
                    }
                    else
                    {
                        OnFinish(mMax);
                    }
                    break;
                case FadeState.FadeOut:
                    if (!mInit)
                    {
                        mCurrentValue = mMax;
                        mInit = true;
                    }
                    if (mCurrentValue > mMin)
                    {
                        mCurrentValue -= step;
                    }
                    else
                    {
                        OnFinish(mMin);
                    }
                    break;
            }
            return mCurrentValue;
        }
    }
}


