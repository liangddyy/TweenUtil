using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TN;

public class MonoTweener : MonoBehaviour
{
    public bool AutoPlay;

    [HideInInspector] [SerializeField] private List<TweenScript> tweeners;
    

    public List<TweenScript> Tweeners
    {
        get { return tweeners; }
        set { tweeners = value; }
    }

    public void Play()
    {
        for (int i = 0; i < Tweeners.Count; i++)
        {
            tweeners[i].Play();
        }
    }

    private void Awake()
    {
        for (int i = 0; i < Tweeners.Count; i++)
        {
            tweeners[i].isPause = true;
            tweeners[i].Init();
        }
        if(AutoPlay)
            Play();
    }

    public void Update()
    {
        for (int i = 0; i < Tweeners.Count; i++)
        {
            Tweeners[i].executeUpdate();
        }
    }
}