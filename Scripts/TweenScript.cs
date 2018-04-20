using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Tween
{
    public class TweenScript : IStackObject
    {
        #region 参数

        //基本变量
        public GameObject animGameObject;

        public AnimType animType;
        public PathType pathType = PathType.Line;
        public bool isDone = false;
        public float currentTime = 0;
        public float totalTime = 0;
        private float currentPercentage; // 可不用初始化或重置


        //V3
        public Vector3 fromV3;

        public Vector3 toV3;

        //V2
        public Vector2 fromV2;

        public Vector2 toV2;

        //Float
        public float fromFloat = 0;

        public float toFloat = 0;

        //Move To(优先 toV3)
        public Transform toTransform;

        //Color
        public Color fromColor;

        public Color toColor;

        List<Color> m_oldColor = new List<Color>();

        //闪烁
        public float blinkTime = 0;

        float blinkCurrentTime = 0;

        //其他设置
        public bool isChild = false;

        public bool isLocal = false;

        public bool isPause = false;

        //路径
        public Vector3[] pathPoints = null; //路径

        private float[] pathWeith;

        private int currentStep = 0;

        //自定义函数
        public AnimCustomMethodVector3 customMethodV3;

        public AnimCustomMethodVector2 customMethodV2;

        public AnimCustomMethodFloat customMethodFloat;

        //缓存变量
        RectTransform m_rectRransform;

        Transform m_transform;

        #endregion

        #region 非必须初始化变量/动画循环/延迟/缓动等。

        private bool isRecyclable = true;
        private object[] animParameter;        //动画回调参数
        private AnimCallBack animCallBack;
        private float delayTime = 0;
        private bool isIgnoreTimeScale = false;
        private Ease easeType = Ease.Linear;
        private int loopCount = -1; // 动画重复次数
        private LoopType loopType = LoopType.Once;
        private AnimationCurve curve;

        /// <summary>
        /// </summary>
        /// <param name="isable">false 不回收脚本</param>
        /// <returns></returns>
        public TweenScript SetRecyclable(bool isable)
        {
            isRecyclable = isable;
            return this;
        }

        public TweenScript SetCallBack(AnimCallBack callBack, object[] parameter = null)
        {
            animCallBack = callBack;
            animParameter = parameter;
            return this;
        }

        public TweenScript SetLoopType(LoopType type, int loopCount = -1)
        {
            loopType = type;
            this.loopCount = loopCount;
            return this;
        }

        public TweenScript SetIgnoreIimeScale(bool ignore)
        {
            isIgnoreTimeScale = ignore;
            return this;
        }

        public TweenScript SetDelay(float delay)
        {
            delayTime = delay;
            return this;
        }

        public TweenScript SetEase(Ease ease)
        {
            easeType = ease;
            return this;
        }

        public TweenScript SetEase(AnimationCurve animationCurve)
        {
            easeType = Ease.Default;
            curve = animationCurve;
            return this;
        }

        #endregion

        #region 核心函数

        public void executeUpdate()
        {
            if (isPause || isDone)
            {
                return;
            }
            if (delayTime <= 0)
            {
                if (isIgnoreTimeScale)
                {
                    currentTime += Time.unscaledDeltaTime;
                }
                else
                {
                    currentTime += Time.deltaTime;
                }
            }
            else
            {
                if (isIgnoreTimeScale)
                {
                    delayTime -= Time.unscaledDeltaTime;
                }
                else
                {
                    delayTime -= Time.deltaTime;
                }
            }
            if (currentTime >= totalTime)
            {
                currentTime = totalTime;
                isDone = true;
            }
            try
            {
                switch (animType)
                {
                    case AnimType.UGUI_Color:
                        UguiColor();
                        break;
                    case AnimType.UGUI_Alpha:
                        UguiAlpha();
                        break;
                    case AnimType.UGUI_AnchoredPosition:
                        UguiPosition();
                        break;
                    case AnimType.UGUI_AnchoredRotate:
                        UguiRotate();
                        break;
                    case AnimType.UGUI_AnchoredLocalRotate:
                        UguiRotate();
                        break;
                    case AnimType.UGUI_AnchoredScale:
                        UguiScale();
                        break;
                    case AnimType.UGUI_Size:
                        SizeDelta();
                        break;
                    case AnimType.Position:
                        Position();
                        break;
                    case AnimType.LocalPosition:
                        LocalPosition();
                        break;
                    case AnimType.LocalScale:
                        LocalScale();
                        break;
                    case AnimType.LocalRotate:
                        LocalRotate();
                        break;
                    case AnimType.Rotate:
                        Rotate();
                        break;
                    case AnimType.Color:
                        UpdateColor();
                        break;
                    case AnimType.Alpha:
                        UpdateAlpha();
                        break;
                    case AnimType.Custom_Vector3:
                        CustomMethodVector3();
                        break;
                    case AnimType.Custom_Vector2:
                        CustomMethodVector2();
                        break;
                    case AnimType.Custom_Float:
                        CustomMethodFloat();
                        break;
                    case AnimType.Blink:
                        Blink();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("TweenUtil Error Exception: " + e.ToString());
            }
        }

        //动画播放完毕执行回调
        public void executeCallBack()
        {
            try
            {
                if (animCallBack != null)
                {
                    animCallBack(animParameter);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public void Pause()
        {
            isPause = true;
        }

        public void Play()
        {
            isPause = false;
        }

        public void Restart()
        {
            isPause = false;
            isDone = false;
            currentTime = 0;
            ResetPathInfo();
        }

        public void Reverse()
        {
            currentTime = totalTime - currentTime;
            PingpangExchange();
            isDone = false;
        }

        /// <summary>
        /// 是否继续循环播放动画
        /// false 将回收脚本
        /// </summary>
        /// <returns></returns>
        public bool AnimReplayLogic()
        {
            switch (loopType)
            {
                case LoopType.Once:
                    ResetPathInfo();
                    return !isRecyclable; // 回收脚本与否
                case LoopType.Loop:
                    Restart();
                    break;
                case LoopType.PingPang:
                    currentTime = 0;
                    isDone = false;
                    PingpangExchange();
                    break;
            }
            if (loopCount == -1)
            {
                return true;
            }
            else
            {
                loopCount--;
                if (loopCount > 0) return true;
                return !isRecyclable;
            }
        }

        private void PingpangExchange()
        {
            ExchangeV2();
            ExchangeColor();
            ExchangeAlpha();
            ExchangePos();
            ResetPathInfo(true);
        }

        #region 循环逻辑

        public void ResetPathInfo(bool isExchange = false)
        {
            if (pathType == PathType.Line)
            {
                return;
            }
            currentStep = 0;
            if (isExchange)
            {
                Array.Reverse(pathPoints);
                InitPathWeight();
                return;
            }
            if (pathWeith.Length > 1)
            {
                // 重置已插值量
                pathWeith[pathWeith.Length - 1] = 0;
            }
        }

        public void ExchangeV2()
        {
            Vector2 Vtmp = fromV2;
            fromV2 = toV2;
            toV2 = Vtmp;
        }

        public void ExchangePos()
        {
            Vector3 Vtmp = fromV3;
            fromV3 = toV3;
            toV3 = Vtmp;
        }

        public void ExchangeColor()
        {
            if (animType == AnimType.Color || animType == AnimType.UGUI_Color)
            {
                // Debug.Log("exchangge");
                Color colorTemp = fromColor;
                fromColor = toColor;
                toColor = colorTemp;
            }
        }

        public void ExchangeAlpha()
        {
            float alphaTmp = fromFloat;
            fromFloat = toFloat;
            toFloat = alphaTmp;
        }

        #endregion

        #endregion

        #region 初始化/公共

        public void Init()
        {
            switch (animType)
            {
                case AnimType.UGUI_Color:
                    UguiColorInit(isChild);
                    break;
                case AnimType.UGUI_Alpha:
                    UguiAlphaInit(isChild);
                    break;
                case AnimType.UGUI_AnchoredPosition:
                case AnimType.UGUI_AnchoredRotate:
                case AnimType.UGUI_AnchoredLocalRotate:
                case AnimType.UGUI_AnchoredScale:
                    UguiAnchoredInit();
                    break;
                case AnimType.UGUI_Size:
                    UguiAnchoredInit();
                    break;
                case AnimType.Color:
                    ColorInit(isChild);
                    break;
                case AnimType.Alpha:
                    AlphaInit(isChild);
                    break;
                case AnimType.Position:
                    TransfromInit();
                    break;
                case AnimType.LocalPosition:
                    TransfromInit();
                    break;
                case AnimType.LocalScale:
                    TransfromInit();
                    break;
                case AnimType.LocalRotate:
                    TransfromInit();
                    break;
                case AnimType.Rotate:
                    TransfromInit();
                    break;
            }
            InitPathWeight();
        }

        private void InitPathWeight()
        {
            currentStep = 0;
            if (pathType == PathType.PathLinear)
            {
                float part = 20;
                pathWeith = new float[pathPoints.Length];
                float sum = 0;
                Vector3 oldVector3 = Vector3.zero, nowVector3 = Vector3.zero;
                for (int i = 0; i < pathPoints.Length - 1; i++)
                {
                    pathWeith[i] = 0; //此时保存长度
                    oldVector3 = TweenMath.CatmullRomPoint(pathPoints[LimitRangeInt(i - 1, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(i, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(i + 1, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(i + 2, pathPoints.Length - 1)], 0);
                    for (int j = 1; j <= part; j++)
                    {
                        nowVector3 = TweenMath.CatmullRomPoint(pathPoints[LimitRangeInt(i - 1, pathPoints.Length - 1)],
                            pathPoints[LimitRangeInt(i, pathPoints.Length - 1)],
                            pathPoints[LimitRangeInt(i + 1, pathPoints.Length - 1)],
                            pathPoints[LimitRangeInt(i + 2, pathPoints.Length - 1)], j / part);
                        pathWeith[i] += Vector3.Distance(oldVector3, nowVector3);
                        oldVector3 = nowVector3;
                    }
                    sum += pathWeith[i];
                }
                for (int i = 0; i < pathWeith.Length - 1; i++)
                {
                    pathWeith[i] = pathWeith[i] / sum; // 插值百分比
                }
                pathWeith[pathWeith.Length - 1] = 0; //保存已插值完成的百分比
            }
            if (pathType == PathType.PathBreak)
            {
                // 根据距离分配时间
                pathWeith = new float[pathPoints.Length];
                float sum = 0;
                for (int i = 0; i < pathPoints.Length - 1; i++)
                {
                    pathWeith[i] = Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
                    sum += pathWeith[i];
                }
                for (int i = 0; i < pathWeith.Length - 1; i++)
                {
                    pathWeith[i] = pathWeith[i] / sum; // 插值百分比
                }
                pathWeith[pathWeith.Length - 1] = 0; //已插值完成的百分比
            }
        }

        /// <summary>
        /// 当脚本初始化时
        /// </summary>
        public void OnInit()
        {
        }

        public void OnPop()
        {
        }

        /// <summary>
        /// 回收脚本时
        /// 初始化
        /// </summary>
        public void OnPush()
        {
            loopType = LoopType.Once;
            easeType = Ease.Linear;
            isPause = false;
            isIgnoreTimeScale = false;
            delayTime = 0;
            isDone = false;
            currentTime = 0;
            totalTime = 0;
            loopCount = -1;
            pathType = PathType.Line;
            pathPoints = null;
            currentStep = 0;
            //            m_floatContral = null;
            //toTransform = null;
        }

        #endregion

        #region CustomMethod

        public void CustomMethodFloat()
        {
            if (customMethodFloat != null)
                customMethodFloat(GetInterpValue(fromFloat, toFloat));
        }

        public void CustomMethodVector2()
        {
            if (customMethodV2 != null)
                customMethodV2(GetInterpV3(fromV2, toV2));
        }

        public void CustomMethodVector3()
        {
            if (customMethodV3 != null)
                customMethodV3(GetInterpV3(fromV3, toV3));
        }

        #endregion

        #region UGUI

        #region UGUI_Color

        List<Image> m_animObjectList_Image = new List<Image>();
        List<Text> m_animObjectList_Text = new List<Text>();

        #region ALpha

        public void UguiAlphaInit(bool isChild)
        {
            // Debug.Log(animGameObject.name);
            m_animObjectList_Image.Clear();
            m_animObjectList_Text.Clear();
            m_oldColor.Clear();
            if (isChild)
            {
                Image[] images = animGameObject.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    if (images[i].transform.GetComponent<Mask>() == null)
                    {
                        m_animObjectList_Image.Add(images[i]);
                        m_oldColor.Add(images[i].color);
                    }
                }
                Text[] texts = animGameObject.GetComponentsInChildren<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    m_animObjectList_Text.Add(texts[i]);
                    m_oldColor.Add(texts[i].color);
                }
            }
            else
            {
                Image image = animGameObject.GetComponent<Image>();
                Text text = animGameObject.GetComponent<Text>();
                if (image != null)
                {
                    m_animObjectList_Image.Add(image);
                    m_oldColor.Add(image.color);
                }
                if (text != null)
                {
                    m_animObjectList_Text.Add(text);
                    m_oldColor.Add(text.color);
                }
            }
            SetUGUIAlpha(fromFloat);
        }

        void UguiAlpha()
        {
            SetUGUIAlpha(GetInterpValue(fromFloat, toFloat));
        }

        Color colTmp = new Color();

        private void SetUGUIAlpha(float a)
        {
            Color newColor = colTmp;
            int index = 0;
            // Debug.Log("Count:" + m_animObjectList_Text.Count);
            for (int i = 0; i < m_animObjectList_Image.Count; i++)
            {
                newColor = m_oldColor[index];
                newColor.a = a;
                m_animObjectList_Image[i].color = newColor;
                index++;
            }
            for (int i = 0; i < m_animObjectList_Text.Count; i++)
            {
                newColor = m_oldColor[index];
                newColor.a = a;
                m_animObjectList_Text[i].color = newColor;
                index++;
            }
        }

        #endregion

        #region Color

        void UguiColor()
        {
            SetUGUIColor(GetInterpolationColor(fromColor, toColor));
        }

        public void UguiColorInit(bool isChild)
        {
            m_animObjectList_Image.Clear();
            m_animObjectList_Text.Clear();
            if (isChild)
            {
                Image[] images = animGameObject.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    if (images[i].transform.GetComponent<Mask>() == null)
                    {
                        m_animObjectList_Image.Add(images[i]);
                    }
                    else
                    {
                        //Debug.LogError("name:" + images[i].gameObject.name);
                    }
                }
                Text[] texts = animGameObject.GetComponentsInChildren<Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    m_animObjectList_Text.Add(texts[i]);
                }
            }
            else
            {
                Image image = animGameObject.GetComponent<Image>();
                Text text = animGameObject.GetComponent<Text>();
                if (image != null)
                {
                    m_animObjectList_Image.Add(image);
                }
                if (text != null)
                {
                    m_animObjectList_Text.Add(text);
                }
            }
            SetUGUIColor(fromColor);
        }

        void SetUGUIColor(Color color)
        {
            for (int i = 0; i < m_animObjectList_Image.Count; i++)
            {
                m_animObjectList_Image[i].color = color;
            }
            for (int i = 0; i < m_animObjectList_Text.Count; i++)
            {
                m_animObjectList_Text[i].color = color;
            }
        }

        #endregion

        #endregion

        #region UGUI_SizeDelta

        void SizeDelta()
        {
            if (m_rectRransform == null)
            {
                Debug.LogError(m_transform.name + "缺少RectTransform组件，不能进行sizeDelta变换！！");
                return;
            }
            m_rectRransform.sizeDelta = GetInterpV3(fromV2, toV2);
        }

        #endregion

        #region UGUI_Position

        public void UguiAnchoredInit()
        {
            m_rectRransform = animGameObject.GetComponent<RectTransform>();
        }

        void UguiPosition()
        {
            m_rectRransform.anchoredPosition3D = GetInterpV3(fromV3, toV3);
        }

        void UguiRotate()
        {
            m_rectRransform.eulerAngles = GetInterpV3(fromV3, toV3);
        }

        void UguiLocalRotate()
        {
            m_rectRransform.localEulerAngles = GetInterpV3(fromV3, toV3);
        }

        void UguiScale()
        {
            m_rectRransform.localScale = GetInterpV3(fromV3, toV3);
        }

        #endregion

        #endregion

        #region Transfrom

        public void TransfromInit()
        {
            m_transform = animGameObject.transform;
        }

        void Position()
        {
            if (toTransform != null)
            {
                m_transform.position = GetInterpV3(fromV3, toTransform.position);
            }
            else
            {
                m_transform.position = GetInterpV3(fromV3, toV3);
            }
        }

        void LocalPosition()
        {
            m_transform.localPosition = GetInterpV3(fromV3, toV3);
        }

        void LocalRotate()
        {
            m_transform.localEulerAngles = GetInterpV3(fromV3, toV3);
        }

        void Rotate()
        {
            m_transform.eulerAngles = GetInterpV3(fromV3, toV3);
        }

        void LocalScale()
        {
            m_transform.localScale = GetInterpV3(fromV3, toV3);
        }

        #endregion

        #region Color

        List<SpriteRenderer> m_animObjectList_Sprite = new List<SpriteRenderer>();

        #region ALPHA

        public void AlphaInit(bool isChild)
        {
            m_animObjectList_Sprite.Clear();
            m_oldColor.Clear();
            if (isChild)
            {
                SpriteRenderer[] images = animGameObject.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < images.Length; i++)
                {
                    m_animObjectList_Sprite.Add(images[i]);
                    m_oldColor.Add(images[i].color);
                }
            }
            else
            {
                SpriteRenderer image = animGameObject.GetComponent<SpriteRenderer>();
                if (image != null)
                {
                    m_animObjectList_Sprite.Add(image);
                    m_oldColor.Add(image.color);
                }
            }
            SetAlpha(fromFloat);
        }

        void UpdateAlpha()
        {
            SetAlpha(GetInterpValue(fromFloat, toFloat));
        }

        private void SetAlpha(float a)
        {
            Color newColor = new Color();
            int index = 0;
            for (int i = 0; i < m_animObjectList_Sprite.Count; i++)
            {
                newColor = m_oldColor[index];
                newColor.a = a;
                Debug.Log(a);
                m_animObjectList_Sprite[i].color = newColor;
                index++;
            }
        }

        #endregion

        #region Color

        public void ColorInit(bool isChild)
        {
            m_animObjectList_Sprite.Clear();
            m_oldColor.Clear();
            if (isChild)
            {
                SpriteRenderer[] images = animGameObject.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < images.Length; i++)
                {
                    m_animObjectList_Sprite.Add(images[i]);
                }
            }
            else
            {
                SpriteRenderer image = animGameObject.GetComponent<SpriteRenderer>();
                if (image != null)
                {
                    m_animObjectList_Sprite.Add(image);
                }
            }
            SetColor(fromColor);
        }

        void UpdateColor()
        {
            SetColor(GetInterpolationColor(fromColor, toColor));
        }

        Color GetInterpolationColor(Color oldValue, Color aimValue)
        {
            Color result = new Color(GetInterpValue(oldValue.r, aimValue.r),
                GetInterpValue(oldValue.g, aimValue.g),
                GetInterpValue(oldValue.b, aimValue.b),
                GetInterpValue(oldValue.a, aimValue.a));
            return result;
        }

        private void SetColor(Color color)
        {
            for (int i = 0; i < m_animObjectList_Sprite.Count; i++)
            {
                m_animObjectList_Sprite[i].color = color;
            }
        }

        #endregion

        #endregion

        #region 闪烁

        void Blink()
        {
            if (blinkCurrentTime < 0)
            {
                blinkCurrentTime = blinkTime;
                animGameObject.SetActive(!animGameObject.activeSelf);
            }
            else
            {
                blinkCurrentTime -= Time.deltaTime;
            }
        }

        #endregion

        #region 线性插值入口

        float GetInterpValue(float oldValue, float aimValue)
        {
            return GetInterp(oldValue, aimValue, currentTime, totalTime);
        }
        /// <summary>
        /// 曲线 缓动 插值时间
        /// </summary>
        /// <returns></returns>
        float GetInterpTimePercentage()
        {
            return GetInterp(0, 1, currentTime / totalTime, 1);
        }

        /// <summary>
        /// 插值入口
        /// </summary>
        /// <returns></returns>
        float GetInterp(float fromValue, float aimValue, float current, float total)
        {
            switch (easeType)
            {
                // 线性
                case Ease.Linear:
                    return Mathf.Lerp(fromValue, aimValue, current / total);
                case Ease.InBack:
                    return TweenMath.InBack(fromValue, aimValue, current, total);
                case Ease.OutBack:
                    return TweenMath.OutBack(fromValue, aimValue, current, total);
                case Ease.InOutBack:
                    return TweenMath.InOutBack(fromValue, aimValue, current, total);
                case Ease.OutInBack:
                    return TweenMath.OutInBack(fromValue, aimValue, current, total);
                case Ease.InQuad:
                    return TweenMath.InQuad(fromValue, aimValue, current, total);
                case Ease.OutQuad:
                    return TweenMath.OutQuad(fromValue, aimValue, current, total);
                case Ease.InoutQuad:
                    return TweenMath.InoutQuad(fromValue, aimValue, current, total);
                case Ease.InCubic:
                    return TweenMath.InCubic(fromValue, aimValue, current, total);
                case Ease.OutCubic:
                    return TweenMath.OutCubic(fromValue, aimValue, current, total);
                case Ease.InoutCubic:
                    return TweenMath.InoutCubic(fromValue, aimValue, current, total);
                case Ease.InQuart:
                    return TweenMath.InQuart(fromValue, aimValue, current, total);
                case Ease.OutQuart:
                    return TweenMath.OutQuart(fromValue, aimValue, current, total);
                case Ease.InOutQuart:
                    return TweenMath.InOutQuart(fromValue, aimValue, current, total);
                case Ease.OutInQuart:
                    return TweenMath.OutInQuart(fromValue, aimValue, current, total);
                case Ease.InQuint:
                    return TweenMath.InQuint(fromValue, aimValue, current, total);
                case Ease.OutQuint:
                    return TweenMath.OutQuint(fromValue, aimValue, current, total);
                case Ease.InOutQuint:
                    return TweenMath.InOutQuint(fromValue, aimValue, current, total);
                case Ease.OutInQuint:
                    return TweenMath.OutInQuint(fromValue, aimValue, current, total);
                case Ease.InSine:
                    return TweenMath.InSine(fromValue, aimValue, current, total);
                case Ease.OutSine:
                    return TweenMath.OutSine(fromValue, aimValue, current, total);
                case Ease.InOutSine:
                    return TweenMath.InOutSine(fromValue, aimValue, current, total);
                case Ease.OutInSine:
                    return TweenMath.OutInSine(fromValue, aimValue, current, total);
                case Ease.InExpo:
                    return TweenMath.InExpo(fromValue, aimValue, current, total);
                case Ease.OutExpo:
                    return TweenMath.OutExpo(fromValue, aimValue, current, total);
                case Ease.InOutExpo:
                    return TweenMath.InOutExpo(fromValue, aimValue, current, total);
                case Ease.OutInExpo:
                    return TweenMath.OutInExpo(fromValue, aimValue, current, total);
                case Ease.InBounce:
                    return TweenMath.InBounce(fromValue, aimValue, current, total);
                case Ease.OutBounce:
                    return TweenMath.OutBounce(fromValue, aimValue, current, total);
                case Ease.InOutBounce:
                    return TweenMath.InOutBounce(fromValue, aimValue, current, total);
                case Ease.OutInBounce:
                    return TweenMath.OutInBounce(fromValue, aimValue, current, total);
                // animationcurve
                case Ease.Default:
                    return curve.Evaluate(current / total) * (aimValue - fromValue) + fromValue;
            }
            return 0;
        }

        Vector3 GetInterpV3(Vector3 oldValue, Vector3 aimValue)
        {
            Vector3 result = Vector3.zero;
            if (pathType == PathType.Line)
            {
                result = new Vector3(
                    GetInterpValue(oldValue.x, aimValue.x),
                    GetInterpValue(oldValue.y, aimValue.y),
                    GetInterpValue(oldValue.z, aimValue.z)
                );
            }
            else
            {
                // 曲线运动插值 percentage 插值进度
                currentPercentage = GetInterpTimePercentage();
                // Debug.Log(currentPercentage);
                result = GetInterpPathV3();
            }
            return result;
        }

        private Vector3 GetInterpPathV3()
        {
            float t = currentPercentage - pathWeith[pathWeith.Length - 1];
            if (t >= pathWeith[currentStep])
            {
                pathWeith[pathWeith.Length - 1] += pathWeith[currentStep];
                t -= pathWeith[currentStep];
                ++currentStep;
            }
            t /= pathWeith[currentStep];
            switch (pathType)
            {
                case PathType.PathBreak:
                    return Vector3.Lerp(pathPoints[LimitRangeInt(currentStep, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(currentStep + 1, pathPoints.Length - 1)], t); // 线性插值
                case PathType.PathLinear:
                    return TweenMath.CatmullRomPoint(pathPoints[LimitRangeInt(currentStep - 1, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(currentStep, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(currentStep + 1, pathPoints.Length - 1)],
                        pathPoints[LimitRangeInt(currentStep + 2, pathPoints.Length - 1)], t);
                default:
                    return Vector3.zero;
            }
        }

        // 防止越界
        private int LimitRangeInt(int t, int max, int min = 0)
        {
            if (t < 0 | max <= 0)
            {
                return 0;
            }
            else if (t > max)
            {
                return max;
            }
            else
            {
                return t;
            }
        }

        #endregion
    }
}