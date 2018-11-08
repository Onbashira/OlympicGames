using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;


public static class ControllerFetcher
{
    [SerializeField]
    private static int maxConnectedNum = 0; //現在つながれているコントローラ数
				private static int maxConnectedNumOld = 0;//一つ前につながれていた数
				private static List<string> connectedData = new List<string>();//つながりを設定する
				private const int MAX_SUPPORTED_NUM = 4; //デフォルトで4つ
				
				// Use this for initialization
				public static void Initialize()
    {
								var controllerNames = Input.GetJoystickNames();
        int controllerNum = 0;

								//既に接続されているものから名前があるもの（現在接続されているもの）の数を抽出
								//0は全部の入力のため排除
								for (int i = 1; i < controllerNames.Length; i++)
        {
												if (controllerNames[i] != "")
												{
																++controllerNum;
																continue;
												}
												//Debug.Log("ControllerIndex " + i + "is Missing");
								}
								maxConnectedNumOld = maxConnectedNum;
								maxConnectedNum = controllerNum;
								connectedData = new List<string>(controllerNames);//初期化
				}

    public static uint GetMaxConectedController()
    {
        return (uint)maxConnectedNum;
    }
				public static uint GetMaxConectedControllerOld()
				{
								return (uint)maxConnectedNumOld;
				}

				public static string[] GetConnectedData()
				{
								string[] array = connectedData.ToArray();
								return array;
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
