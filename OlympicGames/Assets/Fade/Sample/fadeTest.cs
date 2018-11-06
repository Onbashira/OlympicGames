using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class fadeTest : MonoBehaviour
{
    [SerializeField]
    Fade fade = null;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (GamePad.GetButtonDown(GamePad.Button.A,GamePad.Index.Any)) {
            Fadeout();
            Debug.Log("Push A");

        }
    }

    public void Fadeout()
    {
        fade.FadeIn(1, () =>
        {
            fade.FadeOut(1, () =>
            {

            });
        });
    }
}
