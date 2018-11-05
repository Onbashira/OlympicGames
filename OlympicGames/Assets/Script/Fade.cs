using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    enum FADE_STATE
    {
        IN = 0,
        NOE,
        OUT,
    }

    [SerializeField]
    private float fadetime = 0.0f;
    [SerializeField]
    [Range(0.0f,1.0f)]
    private float delta = 0.01f;//増分
    [SerializeField]
    private  float maxFadeTime = 5.0f;
    [SerializeField]
    private delegate void FadeUpdater();
    FadeUpdater fadeUpdater;
    [SerializeField]
    private bool fadeOut;
    private 
    // Use this for initialization
    void Start()
    {
        fadeUpdater = FadeInUpdater;
        fadeOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        fadeUpdater();
    }

    public void FadeOut()
    {
        fadeOut = true;
    }

    private void FadeInUpdater()
    {
        fadetime += delta;
        if (fadetime >= maxFadeTime)
        {
            fadetime = maxFadeTime;
            fadeUpdater = FadeNoneUpdater;

        }
    }

    private void FadeNoneUpdater()
    {
        if (fadeOut)
        {
            fadeUpdater = FadeOutUpdater;
            fadeOut = false;
        }
    }

    private void FadeOutUpdater()
    {
        fadetime -= delta;
        if (fadetime <= 0.0f)
        {
            fadetime = 0.0f;
            fadeUpdater = FadeNoneUpdater;
        }
    }
}
