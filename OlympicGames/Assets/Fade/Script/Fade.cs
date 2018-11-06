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
    private bool fadeInIsCompleted { set; get; }
    [SerializeField]
    private bool fadeOutIsCompleted { set; get; }

    // Use this for initialization
    void Start()
    {
        fadeOutIsCompleted = false;
        fadeInIsCompleted = false;
        Init();
        fader.Range = cutoutRange;
    }

    void Init()
    {
        fader = GetComponent<IFade>();
        fadeOutIsCompleted = false;
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
        while (Time.timeSinceLevelLoad <= endTime)
        {
            cutoutRange = 1.0f - (endTime - Time.timeSinceLevelLoad) / time;
            fader.Range = cutoutRange;
            yield return endFrame;
        }
        cutoutRange = 0.0f;
        fadeOutIsCompleted = true;
        fader.Range = cutoutRange;

        //callback
        if (action != null)
        {
            action();

        }
        fadeOutIsCompleted = false;
    }

    IEnumerator FadeInCoroutine(float time, System.Action action)
    {
        float endTime = Time.timeSinceLevelLoad + time * (cutoutRange);
        var endFrame = new WaitForEndOfFrame();
        while (Time.timeSinceLevelLoad <= endTime)
        {
            cutoutRange = (endTime - Time.timeSinceLevelLoad) / time;
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
        fadeInIsCompleted = false;
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

    public bool IsFadeOutCompleted()
    {
        return fadeOutIsCompleted;
    }

    public bool IsFadeInCompleted()
    {
        return fadeInIsCompleted;
    }

}
