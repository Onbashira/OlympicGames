using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSetting : MonoBehaviour
{
				//STOCKの場合の処理
				//残機を設定する　上限値を設定するか？
				[SerializeField]
				private static int remaining;
				private const int k_initial_remaining = 2;//初期化時の残機数
				private const int k_remaining_min = 1;//残機の最低数
				private const int k_remaining_max = 99;     //最大数
				
				//ギミックのありなし
				public static bool is_Gimick = false;
				
				//プレイヤーの情報
				public struct PlayerData
				{
								public int handicap;//プレイヤーのハンデキャップ
								public ColorIndex color;//プレイヤーの色//同色禁止！！
								public int player_number;//プレイヤーの番号
								public bool is_connected;//コントローラーと繋がっているか
				}
				private static int player_num;//プレイヤーの数

				//ここ後でリスト格納すること
				//でないと無駄な入力が多くなってしまう気がする
				
				public static PlayerData[] player_data = new PlayerData[4];//プレイヤー別の情報//後でListに
				public const int k_player_num_max = 4;//プレイヤーの最大数

				//プレイヤーのカラーの順番
				public enum ColorIndex {GREEN, ORANGE,PINK,PURPLE,RED,BLUE,WHITE,YELLOW};
				private const int k_color_num_max = 7;//キャラのカラー最大値
				private const int k_color_num_min = 0;								//カラー最小値

				//初期化関数
				public static void Reset()
				{
								remaining = k_initial_remaining;

								for(int i = 0;i < k_player_num_max; i++)
								{
												player_data[i].handicap = 0;
												player_data[i].color = (ColorIndex)i;
												player_data[i].player_number = i + 1;
								}
				}
				//残機の取得
				public static int GetRemaining()
				{
								return remaining;
				}
				
				//残機数の変更を行う
				public static void ChangeRemaining(int num)
				{
								remaining += num;
								if (remaining < k_remaining_min)
								{
												remaining = k_remaining_min;
												return;
								}
								else if (remaining < k_remaining_max)
								{
												remaining = k_remaining_max;
												return;
								}
				}

				public static void ChangeIsGimick()
				{
								is_Gimick = !is_Gimick;
				}

				public static void ChangePlayerColor(int player_num, int num)
				{
								player_data[player_num].color += num;

								//範囲外に行っていないか
								if ((int)player_data[player_num].color > k_color_num_max)
								{
												player_data[player_num].color = (ColorIndex)k_color_num_min;
								}
								else if ((int)player_data[player_num].color > k_color_num_min)
								{
												player_data[player_num].color = (ColorIndex)k_color_num_max;
								}
								
								bool is_same = false;
								//誰かと同じ色ではないかどうか調べる
								for (int i = 0; i < k_player_num_max; i++)
								{
												if (i == player_num)
												{
																//自分を調べてどうする
																continue;
												}
												if (player_data[player_num].color == player_data[i].color)
												{
																is_same = true;
																break;
												}
								}
								//同じ色のプレイヤーが居たため再帰しましょう
								if(is_same)
								{
												ChangePlayerColor(player_num, num);
								}
				}

				//一フレーム毎に生存を確認する
				private static void LateUpdate()
				{
								//メニュー時のみ動く
								if (SceneManager.GetActiveScene().name == "Menu")
								{
												return;
								}
								//Menu内でのみ処理を行う
								ControllerFetcher.Initialize();

								//コントローラーとの関係を確認
								for (int i = 0; i < k_player_num_max; i++)
								{
												player_data[i].is_connected = ControllerFetcher.GetIsConnected(i);
								}
				}
}
