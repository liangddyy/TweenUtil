using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tween
{
    [AddComponentMenu("Tween/MonoTweener")]
    public class MonoTweener : MonoBehaviour
    {
        #region<Play Setting>

        [System.Serializable]
        public class OnFinish : UnityEvent
        {
        }

        [SerializeField] OnFinish m_onFinish;
        [SerializeField] private LoopType playType = LoopType.Once;
        [SerializeField] private float animationTime = 1.0f;
        [SerializeField] private float delay = 0;
        [SerializeField] private bool playOnAwake;
        [SerializeField] private bool playOnEnable;
        [SerializeField] private bool ignoreTimeScale;

//    private float playTimeNote;
        private bool isPing = true; //true to ping，false to pong

        private float currentTotalTime;

        /// <summary>
        /// 0-1 real value
        /// </summary>
        private float GetCurrentValue()
        {
            if (!isPlaying)
                return 0;

            float curValue = (currentTotalTime - delay) / animationTime;

            if (!isPing)
            {
                curValue = 1 - curValue;
                if (curValue <= 0)
                {
                    StopPlay();
                }
            }
            else
            {
                if (curValue >= 1)
                {
                    StopPlay();
                }
            }

            if (_isRewind)
            {
                curValue = 1 - curValue;
            }

            curValue = curValue < 0 ? 0 : curValue;
            curValue = curValue > 1 ? 1 : curValue;

            return curValue;
        }

        #endregion

        [Range(0f, 1f)] [HideInInspector] [SerializeField]
        float _previewValue;

        public float previewValue
        {
            get { return _previewValue; }
            set
            {
                if (_previewValue != value)
                {
                    if (_previewValue == 0)
                    {
                        Init();
                        Play();
                    }

                    _previewValue = value;
                    PlayAtTime(_previewValue);
                }
            }
        }

        [HideInInspector] [SerializeField] List<TweenScript> tweens = new List<TweenScript>();

        public TweenScript firstTween
        {
            get
            {
                if (tweens.Count > 0)
                    return tweens[0];
                return null;
            }
        }

        bool _isPlaying = false;
        bool _isRewind = false;

        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        void Awake()
        {
            Init();
            if (playOnAwake)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                Play();
            }
        }

        void Init()
        {
            foreach (TweenScript thisTween in tweens)
            {
                thisTween.totalTime = animationTime;
                thisTween.Init();
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (isPlaying)
            {
                currentTotalTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                if (currentTotalTime >= delay)
                {
                    PlayAtTime(GetCurrentValue());
                }
            }
        }

        //Note Animation time
        void ResetNoteTime()
        {
            currentTotalTime = 0;
        }

        public void Play()
        {
            _isRewind = false;
            _isPlaying = true;
            ResetNoteTime();
            currentTotalTime += animationTime;
        }

        public void PlayReverse()
        {
            _isRewind = true;
            _isPlaying = true;
            ResetNoteTime();
            currentTotalTime += animationTime;
        }

        public void PlayAtTime(float curveValue)
        {
            foreach (TweenScript thisTween in tweens)
            {
                thisTween.currentTime = curveValue * animationTime;
                thisTween.Execute();
//                if (curveValue >= thisTween.minAnimationTime && curveValue <= thisTween.maxAnimationTime)
//                {
//                    float thisValue = (curveValue - thisTween.minAnimationTime) /
//                                      (thisTween.maxAnimationTime - thisTween.minAnimationTime);
//                    thisTween.PlayAtTime(thisValue);
//                }
            }
        }

        //Stop Play
        public virtual void StopPlay()
        {
            switch (playType)
            {
                case LoopType.Once:
                    _isPlaying = false;
//                this.enabled = false;
                    m_onFinish.Invoke();
                    break;

                case LoopType.Loop:
                    ResetNoteTime();
                    break;

                case LoopType.PingPang:
                    ResetNoteTime();
                    isPing = !isPing;
                    break;
            }
        }

        public void AddOnFinished(UnityAction call)
        {
            m_onFinish.AddListener(call);
        }

        public void ClearOnFinished()
        {
            m_onFinish.RemoveAllListeners();
        }

        public void SetFirstTweenValue(Vector3 setFromVector, Vector3 setToVector)
        {
            TweenScript thisFirstTween = firstTween;
            if (thisFirstTween != null)
            {
                thisFirstTween.fromV3 = setFromVector;
                thisFirstTween.toV3 = setToVector;
//                thisFirstTween.SetValue(setFromVector, setToVector);
            }
        }

        public void SetFirstTweenValue(Color setFromColor, Color setToColor)
        {
            TweenScript thisFirstTween = firstTween;
            if (thisFirstTween != null)
            {
//                thisFirstTween.SetValue(setFromColor, setToColor);
                thisFirstTween.fromColor = setFromColor;
                thisFirstTween.toColor = setToColor;
            }
        }

        public void SetFirstTweenValue(float setFromFloat, float setToFloat)
        {
            TweenScript thisFirstTween = firstTween;
            if (thisFirstTween != null)
            {
                thisFirstTween.fromFloat = setFromFloat;
                thisFirstTween.toFloat = setToFloat;
//                thisFirstTween.SetValue(setFromFloat, setToFloat);
            }
        }

        public void SetTweenValue(int index, Vector3 setFromVector, Vector3 setToVector)
        {
            if (index < tweens.Count && index >= 0)
            {
                tweens[index].fromV3 = setFromVector;
                tweens[index].toV3 = setToVector;
//                tweens[index].SetValue(setFromVector, setToVector);
            }
        }

        public void SetTweenValue(int index, Color setFromColor, Color setToColor)
        {
            if (index < tweens.Count && index >= 0)
            {
                tweens[index].fromColor = setFromColor;
                tweens[index].toColor = setToColor;
//                tweens[index].SetValue(setFromColor, setToColor);
            }
        }

        public void SetTweenValue(int index, float setFromFloat, float setToFloat)
        {
            if (index < tweens.Count && index >= 0)
            {
                tweens[index].fromFloat = setFromFloat;
                tweens[index].toFloat = setToFloat;
//                tweens[index].SetValue(setFromFloat, setToFloat);
            }
        }

        public TweenScript GetTween(int index)
        {
            if (index < tweens.Count && index >= 0)
            {
                return tweens[index];
            }

            return null;
        }
    }
}