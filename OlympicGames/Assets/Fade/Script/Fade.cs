using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Fade : MonoBehaviour
{
    IFade fader;

    [SerializeField]
    float cutoutRange;

    [SerializeField]
    private bool fadeInIsCompleted;
    [SerializeField]
    private bool fadeOutisCompleted;
    //[SerializeField]
    //private float fadetime = 0.0f;
    //[SerializeField]
    //[Range(0.0f,1.0f)]
    //private float delta = 0.01f;//増分
    //[SerializeField]
    //private  float maxFadeTime = 5.0f;
    //[SerializeField]
    //private delegate void FadeUpdater();
    //FadeUpdater fadeUpdater;

    // Use this for initialization
    void Start()
    {
        //fadeUpdater = FadeInUpdater;
        fadeOutisCompleted = false;
        fadeInIsCompleted = false;
        Init();
        fader.Range = cutoutRange;
    }

    void Init()
    {
        fader = GetComponent<IFade>();
        fadeOutisCompleted = false;
        fadeInIsCompleted = false;
    }

    void OnValidate()
    {
        Init();
        fader.Range = cutoutRange;
    }

    IEnumerator FadeOutCoroutine(float time, System.Action action)
    {
        float endTime = Time.timeSinceLevelLoad + time * (cutoutRange);
        var endFrame = new WaitForEndOfFrame();
        while(Time.timeSinceLevelLoad <= endTime)
        {
            cutoutRange = (endTime - Time.timeSinceLevelLoad) / time;
            fader.Range = cutoutRange;
            yield return endFrame;
        }
        cutoutRange = 0;
        fadeOutisCompleted = true;
        fader.Range = cutoutRange;
        
        //callback
        if (action != null)
        {
            action();
        }
    }

    IEnumerator FadeInCoroutine(float time, System.Action action)
    {
        float endTime = Time.timeSinceLevelLoad + time * (cutoutRange);
        var endFrame = new WaitForEndOfFrame();
        while (Time.timeSinceLevelLoad <= endTime)
        {
            cutoutRange = 1.0f - (endTime - Time.timeSinceLevelLoad) / time;
            fader.Range = cutoutRange;
            yield return endFrame;
        }
        cutoutRange = 1.0f;
        fadeInIsCompleted = true;
        fader.Range = cutoutRange;

        //callback
        if (action != null)
        {
            action();
        }
    }

    public Coroutine FadeOut(float time, System.Action action)
    {
        StopAllCoroutines();
        return StartCoroutine(FadeOutCoroutine(time, action));
    }

    public Coroutine FadeOut(float time)
    {
        return FadeOut(time, null);
    }

    public Coroutine FadeIn(float time, System.Action action)
    {
        StopAllCoroutines();
        return StartCoroutine(FadeInCoroutine(time, action));
    }

    public Coroutine FadeIn(float time)
    {
        return FadeIn(time, null);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    fadeUpdater();
    //}

    //public void FadeOut()
    //{
    //    fadeOutisCompleted = true;
    //}

    //public bool IsFadeOutCompleted()
    //{
    //    return fadeOutisCompleted;
    //}

    //public bool IsFadeInCompleted()
    //{
    //    return fadeInIsCompleted;
    //}

    //private void FadeInUpdater()
    //{
    //    fadetime += delta;
    //    if (fadetime >= maxFadeTime)
    //    {
    //        fadetime = maxFadeTime;
    //        fadeUpdater = FadeNoneUpdater;

    //    }
    //}

    //private void FadeNoneUpdater()
    //{
    //    if (fadeOutisCompleted)
    //    {
    //        fadeUpdater = FadeOutUpdater;
    //        fadeOutisCompleted = false;
    //    }
    //}

    //private void FadeOutUpdater()
    //{
    //    fadetime -= delta;
    //    if (fadetime <= 0.0f)
    //    {
    //        fadetime = 0.0f;
    //        fadeUpdater = FadeNoneUpdater;
    //    }
    //}
}
