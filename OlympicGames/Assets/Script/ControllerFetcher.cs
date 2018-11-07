using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;


public static class ControllerFetcher
{

    [SerializeField]
    private static int maxConnectedNum = 0; //現在つながれているコントローラ数

    private const int MAX_SUPPORTED_NUM = 4; //デフォルトで4つ

				private static bool[] isConnected;

    // Use this for initialization
    public static void Initialize()
    {
        var controllerNames = Input.GetJoystickNames();
        int controllerNum = 0;
        //既に接続されているものから名前があるもの（現在接続されているもの）の数を抽出
        for (int i = 0; i < MAX_SUPPORTED_NUM; ++i)
        {
            if (controllerNames[i] != "")
            {
                ++controllerNum;
																isConnected[i] = true;
																continue;
            }
            Debug.Log("ControllerIndex " + i + "is Missing");
												isConnected[i] = false;
								}
        maxConnectedNum = controllerNum;
    }

    public static uint GetMaxConectedController()
    {
        return (uint)maxConnectedNum;
    }

				public static bool GetIsConnected(int playerNo)
				{
								if(playerNo < 0 || playerNo >= MAX_SUPPORTED_NUM)
								{
												Debug.Log("取得しようとしたインデックス値が無効です");
												return false;
								}
								return isConnected[playerNo];
				}

				public static GamepadInput.GamepadState GetPadState(uint playerNo, bool raw = false)
				{
								if (playerNo > maxConnectedNum)
								{
												UnityEngine.Assertions.Assert.IsTrue(playerNo <= maxConnectedNum, "取得しようとしたインデックス値が無効なため");
								}
								return GamepadInput.GamePad.GetState((GamePad.Index)playerNo, raw);
				}				
}
