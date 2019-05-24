using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tween
{
    [AddComponentMenu("Tween/MonoTweener")]
    public class MonoTweener : MonoBehaviour
    {
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
                thisTween.Init(gameObject, animationTime, delay);
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
                thisTween.Execute(curveValue);
            }
        }

        //Stop Play
        public virtual void StopPlay()
        {
            switch (playType)
            {
                case LoopType.Once:
                    _isPlaying = false;
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