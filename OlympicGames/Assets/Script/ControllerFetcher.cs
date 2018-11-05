using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;


public static class ControllerFetcher
{

    [SerializeField]
    private static int maxConnectedNum = 0; //現在つながれているコントローラ数

    private static int maxSupportedNum = 4; //デフォルトで4つ

    // Use this for initialization
    public static void Initialize()
    {

        var controllerNames = Input.GetJoystickNames();
        int controllerNum = 0;
        for (int i = 0; i < maxSupportedNum; ++i)
        {
            if (controllerNames[i] != "")
            {
                ++controllerNum;
                continue;
            }
            Debug.Log("ControllerIndex " + i + "is Missing");
        }
        maxConnectedNum = controllerNum;
    }

    public static uint GetMaxConectedController()
    {
        return (uint)maxConnectedNum;
    }

    public static GamepadInput.GamepadState GetPadState(uint playerNo, bool raw = false)
    {
        if (playerNo < 0 || playerNo > maxConnectedNum)
        {
            Debug.Log("Error");
        }
        return GamepadInput.GamePad.GetState((GamePad.Index)playerNo, raw);
    }
}
