using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//player_numは、配列的な位置とする

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
								public int player_number;//どのコントローラーと繋がっているか
								public bool is_connected;//コントローラーと繋がっているか
				}
				//public static PlayerData[] player_data = new PlayerData[4];//プレイヤー別の情報//後でListに
				public const int k_player_num_max = 4;//プレイヤーの最大数

				private static int player_num;//プレイヤーの数

				[SerializeField]
				public static List<PlayerData> player_data = new List<PlayerData>();
				public static bool[] is_connect = new bool[4];

				//プレイヤーのカラーの順番
				public enum ColorIndex { GREEN, ORANGE, PINK, PURPLE, RED, BLUE, WHITE, YELLOW };
				private const int k_color_num_max = 7;//キャラのカラー最大値
				private const int k_color_num_min = 0;        //カラー最小値


				//初期化関数
				public static void initialized()
				{
								remaining = k_initial_remaining;
								for(int i = 0;i < 4;i++)
								{
												//何ともつながっていない
												is_connect[i] = false;
								}
				}
				//PlayerDataの初期化
				private static PlayerData PlayerDataInitialized()
				{
								PlayerData pd;
								pd.handicap = 0;
								pd.color = ColorIndex.GREEN;
								pd.player_number = 0;
								pd.is_connected = false;
								return pd;
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
								PlayerData pd = player_data[player_num];
								//カラーを変える
								pd.color += num;

								//範囲外に行っていないか
								if ((int)pd.color > k_color_num_max)
								{
												pd.color = (ColorIndex)k_color_num_min;
								}
								else if ((int)pd.color > k_color_num_min)
								{
												pd.color = (ColorIndex)k_color_num_max;
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
												if (pd.color == player_data[i].color)
												{
																is_same = true;
																break;
												}
								}
								player_data[player_num] = pd;

								//同じ色のプレイヤーが居たため再帰しましょう
								if (is_same)
								{
												ChangePlayerColor(player_num, num);
								}
				}

				//コントローラーの繋がりを確認する
				public static void ChackPlayerConnected()
				{
								ControllerFetcher.Initialize();
								
								//コントローラーとの関係を確認
								for (int i = 0; i < k_player_num_max; i++)
								{
												PlayerData pd = PlayerDataInitialized();

												//コントローラーは繋がっていたか
												if (is_connect[i])
												{
																//iコントローラーは、player_controller_num[i]番のプレイヤーと繋がっていた
																for(int j = 0;j < player_data.Count;j ++)
																{
																				if(player_data[j].player_number != i)
																				{
																								continue;
																				}
																				//現在繋がっているプレイヤー
																				pd = player_data[j];
																}
												}

												//現在 i プレイヤーが繋がっているか
												bool is_connected = ControllerFetcher.GetIsConnected(i);

												if (pd.is_connected && is_connected || !pd.is_connected && !is_connected)
												{
																//コントローラーに変更はなかった
																continue;
												}

												pd.is_connected = is_connected;
												//コントローラーの状態に変更はなかったかどうかの確認
												if (pd.is_connected)
												{
																//コントローラーが接続された
																if (player_data.Count > k_player_num_max)
																{
																				//プレイヤーの上限数を超えました//ありえないとは思うが
																				continue;
																}
																Debug.Log("コントローラー" + i + "がプレイヤー"+ (player_data.Count + 1) + "Pとして参加しました");
																pd.player_number = i;//プレイヤーとコントローラーの関連付け
																player_data.Add(pd);//新しいコントローラー追加
																is_connect[i] = true;
																continue;
												}
												else
												{
																//コントローラーとの接続が切れました
																if (player_data.Count == 0)
																{
																				continue;
																}

																for (int j = 0; j < player_data.Count; j++)
																{
																				if (player_data[j].player_number != i)
																				{
																								continue;
																				}
																				//現在繋がっているプレイヤー
																				player_data.RemoveAt(j);
																}

																Debug.Log("コントローラーの接続が切れました(" + i + ")");

																is_connect[i] = false;
												}
												//コントローラーの追加はなかった
								}
				}
}
