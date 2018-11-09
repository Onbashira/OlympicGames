using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinker : MonoBehaviour
{


    public int _blinkTimer = 0;
    public int _timer = 0;
    Text render;
    System.Action updater;
    // Use this for initialization
    void Start()
    {
        render = this.GetComponent<Text>();
        updater = Normal;
    }
    void Normal()
    {
        _blinkTimer = (_blinkTimer + 1) % 60;

        if (_blinkTimer / 30 == 1)
        {
            render.enabled = false;
        }
        else
        {
            render.enabled = true;
        }
        if (GamepadInput.GamePad.GetButtonDown(GamepadInput.GamePad.Button.A, GamepadInput.GamePad.Index.Any))
        {
            updater = None;
        }
    }

    void None()
    {
        _blinkTimer = (_blinkTimer + 20) % 60;

        if (_blinkTimer / 30 == 1)
        {
            render.enabled = false;
        }
        else
        {
            render.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        updater();
    }
}
