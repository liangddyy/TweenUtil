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
                if (TweenUtil.GetInstance().animList[i].animGameObject == trans.gameObject)
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
                if (TweenUtil.GetInstance().animList[i].animGameObject == animGameObject)
                {
                    TweenUtil.GetInstance().animList[i].isPause = isPause;
                    if (isReset)
                    {
                        TweenUtil.GetInstance().animList[i].currentTime = 0;
                        TweenUtil.GetInstance().animList[i].isDone = false;
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
                if (TweenUtil.GetInstance().animList[i].animGameObject == trans.gameObject)
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
            tweenTmp.isLocal = true;
            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.isLocal = true;
            tweenTmp.fromV3 = trans.localPosition;
            tweenTmp.toV3 = to;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnMove(this Transform trans, Vector3 to, float time = 0.5f,
            float delayTime = 0, Transform toTransform = null)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.isLocal = true;
            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.animType = AnimType.Position;
            tweenTmp.fromV3 = trans.position;
            tweenTmp.toV3 = to;
            tweenTmp.toTransform = toTransform;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnRotate(this Transform trans, Vector3 to,
            float time = 0.5f,
            float delayTime = 0,
            bool isLocal = true)
        {

            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.animType = AnimType.Rotate;
            tweenTmp.isLocal = isLocal;
            tweenTmp.fromV3 = isLocal ? trans.localEulerAngles : trans.eulerAngles;
            tweenTmp.toV3 = to;
            tweenTmp.isLocal = isLocal;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnScale(this Transform trans, Vector3 to,
            float time = 0.5f,
            float delayTime = 0)
        {
            Vector3 fromTmp = trans.localScale;
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.animType = AnimType.Scale;
            tweenTmp.fromV3 = fromTmp;
            tweenTmp.toV3 = to;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnColorTo(this Transform trans, Color from, Color to,
            float time = 0.5f,
            float delayTime = 0,
            bool isChild = false)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.animType = AnimType.Color;
            tweenTmp.fromColor = from;
            tweenTmp.toColor = to;
            tweenTmp.isChild = isChild;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnAlphaTo(GameObject animObject, float from, float to,
            float time = 0.5f,
            float delayTime = 0,
            bool isChild = false)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = animObject;
            tweenTmp.animType = AnimType.Alpha;
            tweenTmp.fromFloat = from;
            tweenTmp.toFloat = to;
            tweenTmp.isChild = isChild;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        /// <summary>
        /// 动画过度到目标颜色
        /// </summary>
        public static TweenScript TnUguiColor(this RectTransform rectTrans, Color to,
            float time = 0.5f,
            float delayTime = 0,
            bool isChild = false)
        {
            Color fromTmp = Color.white;
            if (rectTrans.GetComponent<Graphic>() != null)
            {
                fromTmp = rectTrans.GetComponent<Graphic>().color;
            }

            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = rectTrans.gameObject;
            tweenTmp.animType = AnimType.UiColor;
            tweenTmp.fromColor = fromTmp;
            tweenTmp.toColor = to;
            tweenTmp.isChild = isChild;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript TnUguiAlpha(this RectTransform rectTrans, float to,
            float time,
            float delayTime = 0,
            bool isChild = false)
        {
            float fromTmp = 1;
            if (rectTrans.GetComponent<Graphic>() != null)
            {
                fromTmp = rectTrans.GetComponent<Graphic>().color.a;
            }

            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = rectTrans.gameObject;
            tweenTmp.animType = AnimType.UiAlpha;
            tweenTmp.fromFloat = fromTmp;
            tweenTmp.toFloat = to;
            tweenTmp.isChild = isChild;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        /// <summary>
        /// UGUI Move RectTransform .anchoredPosition3D
        /// </summary>
        public static TweenScript TnUguiMove(this RectTransform rectTrans, Vector3 to,
            float time = 0.5f,
            float delayTime = 0)
        {
            Vector3 fromTmp = rectTrans.GetComponent<RectTransform>().anchoredPosition;
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();

            tweenTmp.animGameObject = rectTrans.gameObject;
            tweenTmp.animType = AnimType.UiAnchoredPosition;
            tweenTmp.fromV3 = fromTmp;
            tweenTmp.toV3 = to;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }

        /// <summary>
        /// UGUI RectTransfrom 放大缩小
        /// width/height
        /// </summary>
        public static TweenScript TnUguiSize(this RectTransform rectTrans, Vector2 to,
            float time = 0.5f,
            float delayTime = 0)
        {
            Vector2 fromTmp = rectTrans.GetComponent<RectTransform>().sizeDelta;
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animGameObject = rectTrans.gameObject;
            tweenTmp.animType = AnimType.UiSize;
            tweenTmp.fromV2 = fromTmp;
            tweenTmp.toV2 = to;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
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
            tweenTmp.animType = AnimType.Blink;
            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.blinkTime = space;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
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

            tweenTmp.animType = AnimType.Position;
            if (isLocal)
            {
                tweenTmp.fromV3 = trans.transform.localPosition;
            }
            else
            {
                tweenTmp.fromV3 = trans.transform.position;
            }

            if (path.Length < 2)
            {
                pathType = PathType.Line; //小于1个点。
                Debug.LogError("Path point it's too short ");
            }
            else
            {
                Vector3[] realPath = new Vector3[path.Length + 1];
                realPath[0] = tweenTmp.fromV3;
                for (int i = 0; i < path.Length; i++)
                {
                    realPath[i + 1] = path[i];
                }

                tweenTmp.pathNodes = realPath;
            }

            tweenTmp.animGameObject = trans.gameObject;
            tweenTmp.isLocal = isLocal;
            tweenTmp.pathType = pathType;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;

            tweenTmp.Init();
            TweenUtil.GetInstance().AddTween(tweenTmp);
            return tweenTmp;
        }
    }
}