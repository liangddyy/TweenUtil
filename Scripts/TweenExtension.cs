using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tween
{
    /// <summary>
    /// transfrom组件拓展类
    /// </summary>
    public static class TweenExtension
    {
        /// <summary>
        /// 对象身上的所有动画
        /// </summary>
        /// <param name="trans">The trans.</param>
        public static void TnPause(this Transform trans)
        {
            PauseOrContinue(trans.gameObject, true);
        }

        public static void TnReverse(this Transform trans)
        {
            for (int i = 0; i < TweenUtil.GetInstance().animList.Count; i++)
            {
                if (TweenUtil.GetInstance().animList[i].AnimObject == trans.gameObject)
                {
                    TweenUtil.GetInstance().animList[i].Reverse();
                }
            }
        }

        /// <summary>
        /// 继续播放对象身上的所有动画
        /// </summary>
        /// <param name="trans">The trans.</param>
        public static void TnPlay(this Transform trans)
        {
            PauseOrContinue(trans.gameObject, false);
        }

        /// <summary>
        /// 所有动画立即从头开始
        /// </summary>
        public static void TnRestart(this Transform trans)
        {
            PauseOrContinue(trans.gameObject, isReset: true);
        }

        private static void PauseOrContinue(GameObject animGameObject, bool isPause = true, bool isReset = false)
        {
            for (int i = 0; i < TweenUtil.GetInstance().animList.Count; i++)
            {
                if (TweenUtil.GetInstance().animList[i].AnimObject == animGameObject)
                {
                    TweenUtil.GetInstance().animList[i].Pause(isPause);
                    if (isReset)
                    {
                        TweenUtil.GetInstance().animList[i].Reset();
                    }
                }
            }
        }

        /// <summary>
        /// 停止一个对象身上的所有动画
        /// </summary>
        /// <param name="trans">要停止动画的对象</param>
        /// <param name="isCallBack">是否触发回调</param>
        public static void TnStop(this Transform trans, bool isCallBack = false)
        {
            for (int i = 0; i < TweenUtil.GetInstance().animList.Count; i++)
            {
                if (TweenUtil.GetInstance().animList[i].AnimObject == trans.gameObject)
                {
                    if (isCallBack)
                    {
                        TweenScript dataTmp = TweenUtil.GetInstance().animList[i];
                        dataTmp.executeCallBack();
                    }

                    TweenScript tweenData = TweenUtil.GetInstance().animList[i];
                    TweenUtil.GetInstance().animList.RemoveAt(i);
                    i--;
                    StackObjectPool<TweenScript>.PutObject(tweenData);
                }
            }
        }

        /// <summary>
        /// 动画移动到某位置
        /// </summary>
        /// <returns></returns>
        public static TweenScript TnLocalMove(this Transform trans, Vector3 to, float time = 0.5f,
            float delayTime = 0)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(trans.localPosition, to);
            tweenTmp.isLocal = true;
            tweenTmp.Init(trans.gameObject,AnimType.Position,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnMove(this Transform trans, Vector3 to, float time = 0.5f,
            float delayTime = 0, Transform toTransform = null)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.isLocal = false;
            tweenTmp.SetValue(trans.position, to);
            tweenTmp.toTransform = toTransform;
            tweenTmp.Init(trans.gameObject,AnimType.Position,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnRotate(this Transform trans, Vector3 to,
            float time = 0.5f,
            float delayTime = 0,
            bool isLocal = true)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(isLocal ? trans.localEulerAngles : trans.eulerAngles, to);
            tweenTmp.isLocal = isLocal;
            tweenTmp.Init(trans.gameObject,AnimType.Rotate,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnScale(this Transform trans, Vector3 to,
            float time = 0.5f,
            float delayTime = 0)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(trans.localScale, to);
            tweenTmp.Init(trans.gameObject,AnimType.Scale,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnColorTo(this Transform trans, Color from, Color to,
            float time = 0.5f,
            float delayTime = 0)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(from, to);
            tweenTmp.Init(trans.gameObject,AnimType.Color,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnAlphaTo(this Transform trans, float from, float to,
            float time = 0.5f,
            float delayTime = 0)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(from, to);
            tweenTmp.Init(trans.gameObject,AnimType.Alpha,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        /// <summary>
        /// UGUI Move RectTransform .anchoredPosition3D
        /// </summary>
        public static TweenScript TnAnchoredPosition(this RectTransform rectTrans, Vector3 to,
            float time = 0.5f,
            float delayTime = 0)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(rectTrans.anchoredPosition3D, to);
            
            tweenTmp.Init(rectTrans.gameObject,AnimType.UiAnchoredPosition,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        /// <summary>
        /// UGUI RectTransfrom 放大缩小
        /// width/height
        /// </summary>
        public static TweenScript TnUiSize(this RectTransform rectTrans, Vector2 to,
            float time = 0.5f,
            float delayTime = 0)
        {
            Vector2 fromTmp = rectTrans.sizeDelta;
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(fromTmp, to);
            tweenTmp.Init(rectTrans.gameObject,AnimType.UiSize,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        /// <summary>
        /// 隐藏/显示
        /// </summary>
        public static TweenScript TnBlink(this Transform trans, float space,
            float time = 0.5f,
            float delayTime = 0)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            
            tweenTmp.blinkTime = space;

            tweenTmp.Init(trans.gameObject,AnimType.Blink,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnPathMove(this Transform trans, Vector3[] path,
            float time = 2,
            float delayTime = 0,
            bool isLocal = false,
            PathType pathType = PathType.CatmullRom)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            if (pathType == PathType.Line)
            {
                pathType = PathType.CatmullRom;
            }

            Vector3 fromV3;
            if (isLocal)
            {
                fromV3 = trans.transform.localPosition;
            }
            else
            {
                fromV3 = trans.transform.position;
            }

            if (path.Length < 2)
            {
                pathType = PathType.Line; //小于1个点。
                Debug.LogError("Path point it's too short ");
            }
            else
            {
                Vector3[] realPath = new Vector3[path.Length + 1];
                realPath[0] = fromV3;
                for (int i = 0; i < path.Length; i++)
                {
                    realPath[i + 1] = path[i];
                }

                tweenTmp.pathNodes = realPath;
            }

            tweenTmp.isLocal = isLocal;
            tweenTmp.pathType = pathType;

            tweenTmp.Init(trans.gameObject,AnimType.Position,time,delayTime);
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }
    }
}