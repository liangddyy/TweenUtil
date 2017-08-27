using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
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
        public static TweenScript CustomTweenFloat(AnimCustomMethodFloat method, float from, float to,
            float time = 0.5f,
            float delayTime = 0,
            bool IsIgnoreTimeScale = false,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animType = AnimType.Custom_Float;
            tweenTmp.fromFloat = from;
            tweenTmp.toFloat = to;
            tweenTmp.customMethodFloat = method;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.SetLoopType(repeatType, repeatCount);
            // l_tmp.isIgnoreTimeScale = IsIgnoreTimeScale;
            tweenTmp.Init();
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }
        public static TweenScript CustomTweenVector2(AnimCustomMethodVector2 method, Vector2 from, Vector2 to,
            float time = 0.5f,
            float delayTime = 0,
            bool IsIgnoreTimeScale = false,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animType = AnimType.Custom_Vector2;
            tweenTmp.fromV2 = from;
            tweenTmp.toV2 = to;
            tweenTmp.customMethodV2 = method;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.SetLoopType(repeatType, repeatCount);
            // l_tmp.isIgnoreTimeScale = IsIgnoreTimeScale;
            tweenTmp.Init();
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }
        public static TweenScript CustomTweenVector3(AnimCustomMethodVector3 method, Vector3 from, Vector3 to,
            float time = 0.5f,
            float delayTime = 0,
            bool IsIgnoreTimeScale = false,
            LoopType repeatType = LoopType.Once,
            int repeatCount = -1)
        {
            TweenScript tweenTmp = StackObjectPool<TweenScript>.GetObject();
            tweenTmp.animType = AnimType.Custom_Vector3;
            tweenTmp.fromV3 = from;
            tweenTmp.toV2 = to;
            tweenTmp.customMethodV3 = method;
            tweenTmp.SetDelay(delayTime);
            tweenTmp.totalTime = time;
            tweenTmp.SetLoopType(repeatType, repeatCount);
            // l_tmp.isIgnoreTimeScale = IsIgnoreTimeScale;
            tweenTmp.Init();
            GetInstance().animList.Add(tweenTmp);
            return tweenTmp;
        }
        #endregion
        #region ValueTo
        /*
        public static TweenScript ValueTo(AnimParamHash l_animHash)
        {
            TweenScript l_tmp = l_animHash.GetAnimData();
            l_tmp.Init();
            GetInstance().animList.Add(l_tmp);
            return l_tmp;
        }
        public class AnimParamHash : Dictionary<AnimParamType, object>
        {
            public AnimParamHash(params object[] l_params)
            {
                for (int i = 0; i < l_params.Length; i += 2)
                {
                    this[(AnimParamType)l_params[i]] = l_params[i + 1];
                }
            }
            public AnimParamHash SetData(params object[] l_params)
            {
                Clear();
                for (int i = 0; i < l_params.Length; i += 2)
                {
                    this[(AnimParamType)l_params[i]] = l_params[i + 1];
                }
                return this;
            }
            public TweenScript GetAnimData()
            {
                TweenScript DataTmp = StackObjectPool<TweenScript>.GetObject();
                foreach (var hash in this)
                {
                    AnimParamType l_ParamType = hash.Key;
                    switch (l_ParamType)
                    {
                        //基础参数
                        case AnimParamType.GameObj:
                            DataTmp.animGameObject = (GameObject)hash.Value;
                            break;
                        case AnimParamType.AnimType:
                            DataTmp.animType = (AnimType)hash.Value;
                            break;
                        case AnimParamType.Time:
                            DataTmp.totalTime = (float)hash.Value;
                            break;
                        case AnimParamType.InteType:
                            DataTmp.m_easeType = (Ease)hash.Value;
                            break;
                        case AnimParamType.RepeatType:
                            DataTmp.repeatType = (RepeatType)hash.Value;
                            break;
                        case AnimParamType.RepeatCount:
                            DataTmp.repeatCount = (int)hash.Value;
                            break;
                        case AnimParamType.DelayTime:
                            DataTmp.delayTime = (float)hash.Value;
                            break;
                        //From
                        case AnimParamType.FromV3:
                            DataTmp.fromV3 = (Vector3)hash.Value;
                            break;
                        case AnimParamType.FromV2:
                            DataTmp.fromV2 = (Vector2)hash.Value;
                            break;
                        case AnimParamType.FromColor:
                            DataTmp.fromColor = (Color)hash.Value;
                            break;
                        case AnimParamType.FromFloat:
                            DataTmp.fromFloat = (float)hash.Value;
                            break;
                        //To
                        case AnimParamType.ToV3:
                            DataTmp.toV3 = (Vector3)hash.Value;
                            break;
                        case AnimParamType.ToV2:
                            DataTmp.toV2 = (Vector2)hash.Value;
                            break;
                        case AnimParamType.ToColor:
                            DataTmp.toColor = (Color)hash.Value;
                            break;
                        case AnimParamType.ToFloat:
                            DataTmp.toFloat = (float)hash.Value;
                            break;
                        //动画回调
                        case AnimParamType.CallBack:
                            DataTmp.animCallBack = (AnimCallBack)hash.Value;
                            break;
                        case AnimParamType.CallBackParams:
                            DataTmp.animParameter = (object[])hash.Value;
                            break;
                        //定制函数
                        case AnimParamType.CustomMethodV3:
                            DataTmp.customMethodV3 = (AnimCustomMethodVector3)hash.Value;
                            break;
                        case AnimParamType.CustomMethodV2:
                            DataTmp.customMethodV2 = (AnimCustomMethodVector2)hash.Value;
                            break;
                        case AnimParamType.CustomMethodFloat:
                            DataTmp.customMethodFloat = (AnimCustomMethodFloat)hash.Value;
                            break;
                        //闪烁
                        case AnimParamType.Space:
                            DataTmp.blinkTime = (float)hash.Value;
                            break;
                        //路径运动
                        case AnimParamType.PathType:
                            DataTmp.m_pathType = (PathType)hash.Value;
                            break;
                        case AnimParamType.PathData:
                            DataTmp.pathPoints = (Vector3[])hash.Value;
                            break;
                        //其他设置
                        case AnimParamType.IsIncludeChild:
                            DataTmp.isChild = (bool)hash.Value;
                            break;
                        case AnimParamType.IsLocal:
                            DataTmp.isLocal = (bool)hash.Value;
                            break;
                        case AnimParamType.IsIgnoreTimeScale:
                            DataTmp.isIgnoreTimeScale = (bool)hash.Value;
                            break;
                    }
                }
                return DataTmp;
            }
        }
 */
        #endregion
        #region 功能函数
        /// <summary>
        /// 停止一个动画
        /// </summary>
        /// <param name="animGameObject">要停止的动画</param>
        /// <param name="isCallBack">是否触发回调</param>
        public static void StopAnim(TweenScript tweenData, bool isCallBack = false)
        {
            if (isCallBack)
            {
                tweenData.executeCallBack();
            }
            GetInstance().animList.Remove(tweenData);
            StackObjectPool<TweenScript>.PutObject(tweenData);
        }
        public static void FinishAnim(TweenScript tweenData)
        {
            tweenData.currentTime = tweenData.totalTime;
            tweenData.executeUpdate();
            tweenData.executeCallBack();
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
        public void Update()
        {
            for (int i = 0; i < animList.Count; i++)
            {
                animList[i].executeUpdate();
                if (animList[i].isDone == true)
                {
                    TweenScript tweenTmp = animList[i];
                    if (!tweenTmp.AnimReplayLogic())
                    {
                        animList.Remove(tweenTmp);
                        i--;
                        StackObjectPool<TweenScript>.PutObject(tweenTmp);
                    }
                    tweenTmp.executeCallBack();
                }
            }
        }
        #endregion
    }
    #region 枚举与代理声明
    public delegate void AnimCallBack(params object[] arg);
    public delegate void AnimCustomMethodVector3(Vector3 data);
    public delegate void AnimCustomMethodVector2(Vector2 data);
    public delegate void AnimCustomMethodFloat(float data);
    /// <summary>
    /// 动画类型
    /// </summary>
    public enum AnimType
    {
        LocalPosition,
        Position,
        LocalScale,
        LocalRotate,
        Rotate,
        Color,  // SpriteRenderer.Color
        Alpha,
        UGUI_Color,     // Image.Color  Text.Color
        UGUI_Alpha,
        UGUI_AnchoredPosition,
        UGUI_AnchoredRotate,
        UGUI_AnchoredLocalRotate,
        UGUI_AnchoredScale,
        UGUI_Size,
        Custom_Vector3,
        Custom_Vector2,
        Custom_Float,
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
    /// 动画控制类型
    /// </summary>
    // public enum AnimParamType
    // {
    //     GameObj,
    //     FromV3,
    //     FromV2,
    //     FromFloat,
    //     FromColor,
    //     ToV3,
    //     ToV2,
    //     ToFloat,
    //     ToColor,
    //     DelayTime,
    //     AnimType,
    //     Time,
    //     InteType,
    //     IsIgnoreTimeScale,
    //     PathType,
    //     PathData,
    //     //        floatControl,
    //     IsIncludeChild,
    //     IsLocal,
    //     RepeatType,
    //     RepeatCount,
    //     CustomMethodV3,
    //     CustomMethodV2,
    //     CustomMethodFloat,
    //     Space,
    //     CallBack,
    //     CallBackParams
    // }
    /// <summary>
    /// 路径类型
    /// </summary>
    public enum PathType
    {
        Line,
        PathBreak, 
        PathLinear, 
    }
    public enum LoopType
    {
        Once,
        Loop,
        PingPang,
    }
    #endregion
}