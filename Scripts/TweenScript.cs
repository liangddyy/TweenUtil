using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Tween
{
    [System.Serializable]
    public class TweenScript : IStackObject
    {
        #region 参数

        //基本变量
        [SerializeField] private GameObject animGameObject;
        [SerializeField] private AnimType animType;
        [SerializeField] public PathType pathType = PathType.Line;
        [SerializeField] private bool isDone;

        private float currentTime = 0;
        private float totalTime = 1;
        private float currentPercentage; // 可不用初始化或重置

        public void SetValue(Vector3 from, Vector3 to)
        {
            fromV3 = from;
            toV3 = to;
        }

        public void SetValue(Color from, Color to)
        {
            fromColor = from;
            toColor = to;
        }

        public void SetValue(Vector2 from, Vector2 to)
        {
            fromV2 = from;
            toV2 = to;
        }

        public void SetValue(float from, float to)
        {
            fromFloat = from;
            toFloat = to;
        }

        public GameObject AnimObject
        {
            get { return animGameObject; }
        }

        public AnimType AnimType
        {
            get { return animType; }
        }

        public bool IsDone
        {
            get { return isDone; }
        }

        //V3
        [SerializeField] private Vector3 fromV3;

        [SerializeField] private Vector3 toV3;

        //V2
        [SerializeField] private Vector2 fromV2;

        [SerializeField] private Vector2 toV2;

        //Float
        [SerializeField] private float fromFloat = 0;

        [SerializeField] private float toFloat = 0;

        //Move To(优先 toV3)
        public Transform toTransform;

        //Color
        [SerializeField] private Color fromColor;

        [SerializeField] private Color toColor;

        //闪烁
        public float blinkTime;

        float blinkCurrentTime;

        //其他设置
        public bool isLocal;

        public bool isPause;

        //路径
        public Vector3[] pathNodes; //路径

        private float[] pathWeight; // 路径的权重

        private int currentStep = 0;

        //自定义函数
        public AnimCustomMethodVector3 customMethodV3;

        public AnimCustomMethodVector2 customMethodV2;

        public AnimCustomMethodFloat customMethodFloat;

        //缓存变量
        private RectTransform rectTransform;

        public Transform m_transform;

        #endregion

        #region 非必须初始化变量/动画循环/延迟/缓动等。

        private bool isRecyclable = true;
        private object[] animParameter; //动画回调参数
        private AnimCallBack animCallBack;
        private bool isExcutedCallback;
        private float delayTime = 0;
        private bool isIgnoreTimeScale = false;

        [SerializeField] private Ease easeType = Ease.Linear;
        private int loopCount = -1; // 动画重复次数
        private LoopType loopType = LoopType.Once;
        [SerializeField] private AnimationCurve curve = new AnimationCurve();

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
            isExcutedCallback = false;
            return this;
        }

        public TweenScript SetLoopType(LoopType type, int loopCount = -1)
        {
            loopType = type;
            this.loopCount = loopCount;
            return this;
        }

        public TweenScript SetIgnoreTimeScale(bool ignore)
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

            Execute();
        }

        public void Execute(float value)
        {
            currentTime = value * totalTime;
            Execute();
        }

        private void Execute()
        {
            //            try
//            {
            switch (animType)
            {
                case AnimType.UiAnchoredPosition:
                    UguiPosition();
                    break;
                case AnimType.UiSize:
                    SizeDelta();
                    break;
                case AnimType.Position:
                    Position();
                    break;
                case AnimType.Scale:
                    Scale();
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
                case AnimType.CustomVector3:
                    CustomMethodVector3();
                    break;
                case AnimType.CustomVector2:
                    CustomMethodVector2();
                    break;
                case AnimType.CustomFloat:
                    CustomMethodFloat();
                    break;
                case AnimType.Blink:
                    Blink();
                    break;
            }

//            }
//            catch (Exception e)
//            {
//                Debug.LogError("TweenUtil Error Exception: " + e.ToString());
//            }
        }

        public void FinishAnim()
        {
            currentTime = totalTime;
            executeUpdate();
            executeCallBack();
            TweenUtil.GetInstance().animList.Remove(this);
            StackObjectPool<TweenScript>.PutObject(this);
        }

        //动画播放完毕执行回调
        public void executeCallBack()
        {
            if (isExcutedCallback)
                return;
            isExcutedCallback = true;
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

        public void Pause(bool pause = true)
        {
            isPause = pause;
        }

        public void Play()
        {
            isPause = false;
            isExcutedCallback = false;
        }

        public void Restart()
        {
            isPause = false;
            isDone = false;
            currentTime = 0;
            ResetPathInfo();
            isExcutedCallback = false;
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
                Array.Reverse(pathNodes);
                InitPathWeight();
                return;
            }

            if (pathWeight.Length > 1)
            {
                // 重置已插值量
                pathWeight[pathWeight.Length - 1] = 0;
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
            if (animType == AnimType.Color)
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

        public TweenScript Init(GameObject animObj, AnimType type, float animTime, float delay = 0)
        {
            animType = type;
            return Init(animObj, animTime, delay);
        }

        public TweenScript Init(GameObject animObj, float animTime, float delay = 0)
        {
            animGameObject = animObj;
            totalTime = animTime;
            delayTime = delay;
            try
            {
                switch (animType)
                {
                    case AnimType.UiAnchoredPosition:
                    case AnimType.UiSize:
                        UguiAnchoredInit();
                        break;
                    case AnimType.Color:
                        ColorInit();
                        break;
                    case AnimType.Alpha:
                        AlphaInit();
                        break;
                    case AnimType.Position:
                    case AnimType.Scale:
                    case AnimType.Rotate:
                        TransfromInit();
                        break;
                }

                InitPathWeight();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                isDone = true;
            }

            return this;
        }

        private void InitPathWeight()
        {
            currentStep = 0;
            if (pathType == PathType.CatmullRom)
            {
                float part = 20;
                pathWeight = new float[pathNodes.Length];
                float sum = 0;
                Vector3 oldVector3 = Vector3.zero, nowVector3 = Vector3.zero;
                for (int i = 0; i < pathNodes.Length - 1; i++)
                {
                    pathWeight[i] = 0; //此时保存长度
                    oldVector3 = TweenMath.CatmullRomPoint(pathNodes[LimitRangeInt(i - 1, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(i, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(i + 1, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(i + 2, pathNodes.Length - 1)], 0);
                    for (int j = 1; j <= part; j++)
                    {
                        nowVector3 = TweenMath.CatmullRomPoint(pathNodes[LimitRangeInt(i - 1, pathNodes.Length - 1)],
                            pathNodes[LimitRangeInt(i, pathNodes.Length - 1)],
                            pathNodes[LimitRangeInt(i + 1, pathNodes.Length - 1)],
                            pathNodes[LimitRangeInt(i + 2, pathNodes.Length - 1)], j / part);
                        pathWeight[i] += Vector3.Distance(oldVector3, nowVector3);
                        oldVector3 = nowVector3;
                    }

                    sum += pathWeight[i];
                }

                for (int i = 0; i < pathWeight.Length - 1; i++)
                {
                    pathWeight[i] = pathWeight[i] / sum; // 插值百分比
                }

                pathWeight[pathWeight.Length - 1] = 0; //保存已插值完成的百分比
            }

            if (pathType == PathType.Linear)
            {
                // 根据距离分配时间
                pathWeight = new float[pathNodes.Length];
                float sum = 0;
                for (int i = 0; i < pathNodes.Length - 1; i++)
                {
                    pathWeight[i] = Vector3.Distance(pathNodes[i], pathNodes[i + 1]);
                    sum += pathWeight[i];
                }

                for (int i = 0; i < pathWeight.Length - 1; i++)
                {
                    pathWeight[i] = pathWeight[i] / sum; // 插值百分比
                }

                pathWeight[pathWeight.Length - 1] = 0; //已插值完成的百分比
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
            pathNodes = null;
            currentStep = 0;
            //            m_floatContral = null;
            //toTransform = null;
        }

        #endregion

        #region CustomMethod

        private void CustomMethodFloat()
        {
            if (customMethodFloat != null)
                customMethodFloat.Invoke(GetInterpValue(fromFloat, toFloat));
//                customMethodFloat(GetInterpValue(fromFloat, toFloat));
        }

        private void CustomMethodVector2()
        {
            if (customMethodV2 != null)
                customMethodV2.Invoke(GetInterpV3(fromV2, toV2));
//                customMethodV2(GetInterpV3(fromV2, toV2));
        }

        private void CustomMethodVector3()
        {
            if (customMethodV3 != null)
                customMethodV3.Invoke(GetInterpV3(fromV3, toV3));
//                customMethodV3(GetInterpV3(fromV3, toV3));
        }

        #endregion


        #region UGUI_SizeDelta

        void SizeDelta()
        {
            if (rectTransform == null)
            {
                Debug.LogError(m_transform.name + "缺少RectTransform组件，不能进行sizeDelta变换！！");
                return;
            }

            rectTransform.sizeDelta = GetInterpV3(fromV2, toV2);
        }

        #endregion

        #region UGUI_Position

        public void UguiAnchoredInit()
        {
            rectTransform = animGameObject.GetComponent<RectTransform>();
        }

        void UguiPosition()
        {
            rectTransform.anchoredPosition3D = GetInterpV3(fromV3, toV3);
        }

        #endregion


        #region Transfrom

        public void TransfromInit()
        {
            m_transform = animGameObject.transform;
        }

        void Position()
        {
            if (isLocal)
            {
                m_transform.localPosition = GetInterpV3(fromV3, toV3);
            }
            else
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
        }

        void Rotate()
        {
            if (isLocal)
            {
                m_transform.localEulerAngles = GetInterpV3(fromV3, toV3);
            }
            else
            {
                m_transform.eulerAngles = GetInterpV3(fromV3, toV3);
            }
        }

        void Scale()
        {
            m_transform.localScale = GetInterpV3(fromV3, toV3);
        }

        #endregion

        #region Color

        private MaskableGraphic maskableGraphic;
        private Renderer render;
        private SpriteRenderer spriteRender;
        private ParticleSystem particleSystem;

        #region ALPHA

        public void AlphaInit()
        {
            maskableGraphic = animGameObject.GetComponent<MaskableGraphic>();
            render = animGameObject.GetComponent<Renderer>();
            spriteRender = animGameObject.GetComponent<SpriteRenderer>();
            particleSystem = animGameObject.GetComponent<ParticleSystem>();

            SetAlpha(fromFloat);
        }

        void UpdateAlpha()
        {
            SetAlpha(GetInterpValue(fromFloat, toFloat));
        }

        private void SetAlpha(float curAlpha)
        {
            if (maskableGraphic != null)
                maskableGraphic.color = new Color(maskableGraphic.color.r, maskableGraphic.color.g,
                    maskableGraphic.color.b, curAlpha);

            if (spriteRender != null)
            {
                spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b,
                    curAlpha);
            }
            else if (render != null)
            {
                if (render.sharedMaterial != null && render.sharedMaterial.HasProperty("_TintColor"))
                {
                    Color currentColor = render.sharedMaterial.GetColor("_TintColor");
                    Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, curAlpha);
                    render.sharedMaterial.SetColor("_TintColor", newColor);
                }
            }

            if (particleSystem != null)
            {
                var particleMain = particleSystem.main;
                particleMain.startColor = new Color(particleMain.startColor.color.r,
                    particleMain.startColor.color.g, particleMain.startColor.color.b, curAlpha);
            }
        }

        #endregion

        #region Color

        public void ColorInit()
        {
            maskableGraphic = animGameObject.GetComponent<MaskableGraphic>();
            render = animGameObject.GetComponent<Renderer>();
            spriteRender = animGameObject.GetComponent<SpriteRenderer>();
            particleSystem = animGameObject.GetComponent<ParticleSystem>();

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

        private void SetColor(Color curColor)
        {
            if (maskableGraphic != null)
                maskableGraphic.color = curColor;

            if (spriteRender != null)
                spriteRender.color = curColor;
            else if (render != null)
            {
                if (render.sharedMaterial != null)
                    render.sharedMaterial.color = curColor;
            }

            if (particleSystem != null)
            {
                var particleMain = particleSystem.main;
                particleMain.startColor = curColor;
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
                case Ease.Default:
                    return curve.Evaluate(current / total) * (aimValue - fromValue) + fromValue;
                // 线性
                default:
                    return Mathf.Lerp(fromValue, aimValue, current / total);
            }
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
            float t = currentPercentage - pathWeight[pathWeight.Length - 1];
            if (t >= pathWeight[currentStep])
            {
                pathWeight[pathWeight.Length - 1] += pathWeight[currentStep];
                t -= pathWeight[currentStep];
                ++currentStep;
            }

            t /= pathWeight[currentStep];
            switch (pathType)
            {
                case PathType.Linear:
                    return Vector3.Lerp(pathNodes[LimitRangeInt(currentStep, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(currentStep + 1, pathNodes.Length - 1)], t); // 线性插值
                case PathType.CatmullRom:
                    return TweenMath.CatmullRomPoint(pathNodes[LimitRangeInt(currentStep - 1, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(currentStep, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(currentStep + 1, pathNodes.Length - 1)],
                        pathNodes[LimitRangeInt(currentStep + 2, pathNodes.Length - 1)], t);
                default:
                    return Vector3.zero;
            }
        }

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

        public void Reset()
        {
            currentTime = 0;
            isDone = false;
        }
    }
}