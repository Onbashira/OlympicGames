using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;


public static class ControllerManager
{

    [SerializeField]
    private static int maxConnectedNum = 0; //現在つながれているコントローラ数

    private static int maxSupportedNum = 4; //デフォルトで4つ

    // Use this for initialization
    public static void Initialize()
    {
        var controllerNames = Input.GetJoystickNames();
        if (controllerNames[0] == "") Debug.Log("Error");
        maxConnectedNum = controllerNames.Length;
    }

    public static int GetControllerNo(int playerNum)
    {
        if (playerNum < 0 || playerNum > maxConnectedNum)
        {
            Debug.Log("Error");
            return -1;
        }
        return playerNum;
    }
}
