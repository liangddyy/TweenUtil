using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Tween
{
    public class TweenUtil : MonoBehaviour
    {
        #region 静态部分

        static TweenUtil instance;

        // static AnimParamHash HashTemp = new AnimParamHash(); 
        public static TweenUtil GetInstance()
        {
            if (instance == null)
            {
                GameObject animGameObject = new GameObject();
                animGameObject.name = "[TweenUtil]";
                instance = animGameObject.AddComponent<TweenUtil>();
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
                else
                {
                    EditorApplication.update += instance.Update;
                }
#else
             DontDestroyOnLoad(instance.gameObject);
#endif
            }

            return instance;
        }

        #region CustomTween

        public static TweenScript CustomTweenFloat(UnityAction<float> method, float from, float to,
            float time = 0.5f,
            float delayTime = 0,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(from, to);
            tweenTmp.customMethodFloat = new AnimCustomMethodFloat();
            tweenTmp.customMethodFloat.AddListener(method);
            tweenTmp.SetLoopType(repeatType, repeatCount);
            tweenTmp.Init(null, AnimType.CustomFloat, time, delayTime);
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript CustomTweenVector2(UnityAction<Vector2> method, Vector2 from, Vector2 to,
            float time = 0.5f,
            float delayTime = 0,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(from, to);
            tweenTmp.customMethodV2 = new AnimCustomMethodVector2();
            if (method != null) tweenTmp.customMethodV2.AddListener(method);
            tweenTmp.SetLoopType(repeatType, repeatCount);
            tweenTmp.Init(null, AnimType.CustomVector2, time, delayTime);
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }

        public static TweenScript CustomTweenVector3(UnityAction<Vector3> method, Vector3 from, Vector3 to,
            float time = 0.5f,
            float delayTime = 0,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.SetValue(from, to);
            tweenTmp.customMethodV3 = new AnimCustomMethodVector3();
            if(method!=null)tweenTmp.customMethodV3.AddListener(method);
            tweenTmp.SetLoopType(repeatType, repeatCount);
            tweenTmp.Init(null, AnimType.CustomVector3, time, delayTime);
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }

        #endregion

        #region 功能函数

        public static void StopAnim(TweenScript tweenData, bool isCallBack = false)
        {
            if (isCallBack)
            {
                tweenData.executeCallBack();
            }

            GetInstance().animList.Remove(tweenData);
            StackObjectPool<TweenScript>.PutObject(tweenData);
        }

        public static void ClearAllAnim(bool isCallBack = false)
        {
            if (isCallBack)
            {
                for (int i = 0; i < GetInstance().animList.Count; i++)
                {
                    GetInstance().animList[i].executeCallBack();
                    GetInstance().animList.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                GetInstance().animList.Clear();
            }
        }

        #endregion

        #endregion

        #region 实例部分

        public List<TweenScript> animList = new List<TweenScript>();

        public void AddTween(TweenScript tweenScript)
        {
            // 避免同一物体同一类型同时存在两次.
            var a = animList.Find(x =>
                x.AnimObject == tweenScript.AnimObject && x.AnimType == tweenScript.AnimType);
            if (a != null)
            {
                animList.Remove(a);
                StackObjectPool<TweenScript>.PutObject(a);
            }

            animList.Add(tweenScript);
        }

        public void Update()
        {
            for (int i = 0; i < animList.Count; i++)
            {
                animList[i].executeUpdate();
                if (animList[i].IsDone) 
                {
                    TweenScript tweenTmp = animList[i];
                    if (!tweenTmp.AnimReplayLogic())
                    {
                        animList.Remove(tweenTmp);
                        i--;
                        StackObjectPool<TweenScript>.PutObject(tweenTmp);
                    }

                    tweenTmp.executeCallBack(); // todo this is bug.
                }
            }
        }

        public void Finish()
        {
            
        }

        #endregion
    }

    #region 枚举与代理声明

    public delegate void AnimCallBack(params object[] arg);

//    public delegate void AnimCustomMethodVector3(Vector3 data);
//    public delegate void AnimCustomMethodVector2(Vector2 data);
//    public delegate void AnimCustomMethodFloat(float data);

    /// <summary>
    /// 动画类型
    /// </summary>
    public enum AnimType
    {
        Position,
        Rotate,
        Scale,
        Color, // SpriteRenderer.Color
        Alpha,
//        UiColor, // Image.Color  Text.Color
//        UiAlpha,
        UiAnchoredPosition,
        UiSize,
        CustomVector3,
        CustomVector2,
        CustomFloat,
        Blink,
    }

    /// <summary>
    /// 插值算法类型
    /// </summary>
    public enum Ease
    {
        Default,
        Linear,
        InBack,
        OutBack,
        InOutBack,
        OutInBack,
        InQuad,
        OutQuad,
        InoutQuad,
        InCubic,
        OutCubic,
        InoutCubic,
        OutInCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        OutInQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        OutInQuint,
        InSine,
        OutSine,
        InOutSine,
        OutInSine,
        InExpo,
        OutExpo,
        InOutExpo,
        OutInExpo,
        OutBounce,
        InBounce,
        InOutBounce,
        OutInBounce,
    }

    /// <summary>
    /// 路径类型
    /// </summary>
    public enum PathType
    {
        Line,
        Linear,
        CatmullRom,
    }

    public enum LoopType
    {
        Once,
        Loop,
        PingPang,
    }

    #endregion
}