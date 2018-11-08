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
				private const int k_remaining_max = 5;						//最大数

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
				public static bool[] is_connect = new bool[4];//繋がっているかどうか

				//繋がったタイミングかどうか
				public enum ConnectState { ON, OFF, NON };

				//プレイヤーのカラーの順番
				public enum ColorIndex { GREEN, ORANGE, PINK, PURPLE, RED, BLUE, WHITE, YELLOW };
				private const int k_color_num_max = 7;//キャラのカラー最大値
				private const int k_color_num_min = 0;        //カラー最小値


				//初期化関数
				public static void initialized()
				{
								remaining = k_initial_remaining;
								for (int i = 0; i < 4; i++)
								{
												//何ともつながっていない
												is_connect[i] = false;
								}
								ModeSetting.ConnectedUpdate();
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
								else if (remaining > k_remaining_max)
								{
												remaining = k_remaining_max;
												return;
								}
				}

				public static void ChangePlayerColor(int player_num, int num)
				{
								if (player_data.Count <= player_num)
								{
												//色を変えようとしているプレイヤーは存在しません
												return;
								}
								PlayerData pd = player_data[player_num];
								//カラーを変える
								pd.color += num;

								//範囲外に行っていないか
								if ((int)pd.color > k_color_num_max)
								{
												pd.color = (ColorIndex)k_color_num_min;
								}
								else if ((int)pd.color < k_color_num_min)
								{
												pd.color = (ColorIndex)k_color_num_max;
								}
								bool is_same = false;
								//誰かと同じ色ではないかどうか調べる
								for (int i = 0; i < player_data.Count; i++)
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
				public static void ConnectedUpdate()
				{
								ControllerFetcher.Initialize();

								//一つ前の時の確認と比べてプレイヤーの総数が変わっているかの確認をする
								if (ControllerFetcher.GetMaxConectedController() > ControllerFetcher.GetMaxConectedControllerOld())
								{
												//コントローラーの数が増えた
												PlusPlayer();
								}
								else if (ControllerFetcher.GetMaxConectedController() < ControllerFetcher.GetMaxConectedControllerOld())
								{
												//減った
												DaletePlayer();
								}
								//何も変わらなかった
				}

				private static void PlusPlayer()
				{
								//新しくコントローラーが追加された
								string[] connect_data = ControllerFetcher.GetConnectedData();
								for(int i = 0;i < connect_data.Length; i++)
								{
												if(connect_data[i] == "")
												{
																//なんもつながっとらん
																continue;
												}

												for(int j = 0;j < player_data.Count;j++)
												{
																if( player_data[j].player_number ==  i )
																{
																				//既に繋がっていたコントローラーだった
																				continue;
																}
												}
												//新しい繋がりを確認しました
												CleateNewPlayer(i);
												return;
								}
				}

				private static void DaletePlayer()
				{
								//入力を確認できないコントローラーを確認します
								string[] connect_data = ControllerFetcher.GetConnectedData();

								List<int> connect_chacker = new List<int>();

								for (int i = 0; i < connect_data.Length; i++)
								{
												if (connect_data[i] != "")
												{
																//コントローラーの接続を確認
																continue;
												}

												for (int j = 0; j < player_data.Count; j++)
												{
																if (player_data[j].player_number == i)
																{
																				//既に繋がっていたコントローラーだった
																				connect_chacker.Add(i);
																				continue;
																}
												}
								}
								//確認できなかったコントローラーを削除します
								for(int i = 0;i < player_data.Count;i++)
								{
												for(int j = 0; j < connect_chacker.Count;j++)
												{
																if(player_data[i].player_number == connect_chacker[j])
																{
																				//繋がりありました
																				continue;
																}
																//繋がりを確認
																player_data.RemoveAt(i);
												}
								}
				}

				private static void CleateNewPlayer(int new_number)
				{
								if(new_number == 0)
								{
												//接続0のコントローラーのデータでプレイヤーを作成しません
												return;
								}
								if (player_data.Count >= k_player_num_max)
								{
												//プレイヤーの数が限界突破した
												return;
								}
								PlayerData pd = new PlayerData();
								pd.player_number = new_number;
								player_data.Add(pd);
								ChangePlayerColor(player_data.Count - 1, 1);
				}
}



//				//コントローラーの状態に変更はなかったかどうかの確認
//				if (pd.is_connected)
//				{
//								//コントローラーが接続された
//								if (player_data.Count > k_player_num_max)
//								{
//												//プレイヤーの上限数を超えました//ありえないとは思うが
//												continue;
//								}
//								Debug.Log("コントローラー" + i + "がプレイヤー" + (player_data.Count + 1) + "Pとして参加しました");
//								pd.player_number = i;//プレイヤーとコントローラーの関連付け
//								player_data.Add(pd);//新しいコントローラー追加
//								ChangePlayerColor(player_data.Count - 1, 1);
//								is_connect[i] = true;
//								continue;
//				}
//				else
//				{
//								//コントローラーとの接続が切れました
//								if (player_data.Count == 0)
//								{
//												continue;
//								}
//								for (int j = 0; j < player_data.Count; j++)
//								{
//												if (player_data[j].player_number != i)
//												{
//																continue;
//												}
//												//つながりが切れたプレイヤーデータ
//												player_data.RemoveAt(j);
//								}
//								Debug.Log("コントローラーの接続が切れました(" + i + ")");
//				}
//				//コントローラーの追加はなかった
////}
//private static void PlayerConnectChack(int num)
//{
//				string[] controller_connect = ControllerFetcher.GetConnectedData();
//				List<int> connect_number = new List<int>();//繋がっているコントローラー
//																																															//コントローラーとの関係を確認
//				for (int i = 0; i < controller_connect.Length; i++)
//				{
//								//接続があるかどうか
//								if (controller_connect[i] == "")
//								{
//												continue;
//								}
//								connect_number.Add(i);
//								if (connect_number.Count >= player_data.Count + num)
//								{
//												break;
//								}
//				}

//				int count = 0;

//				//コントローラーがつながっている
//				for (int i = 0; i < player_data.Count; i++)
//				{

//				}
//				if (connect_number.Count == player_data.Count + num)
//				{

//				}
//}