using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSeting : MonoBehaviour
{
				//ゲームモード
				enum GameMode {TIME,STOCK, AMOUNT };
				private static GameMode game_mode;

				//TIMEの場合の処理
				//描画方法を設定
				[SerializeField]
				private static int limit_time;//設定している制限時間
				private const int k_initial_limit_time = 60;//初期化時の制限時間
				private const int k_limit_time_min = 30;//制限時間の最低時間
				private const int k_limit_time_max = 180;         //最大時間

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
				struct PlayerData
				{
								public int handicap;//プレイヤーのハンデキャップ
								public int color;//プレイヤーの色//同色禁止！！
								public int player_number;//プレイヤーの番号
								public bool is_connected;//コントローラーと繋がっているか
				}
				private static int player_num;//プレイヤーの数
				private static PlayerData[] player_data = new PlayerData[4];//プレイヤー別の情報
				public const int k_player_num_max = 4;//プレイヤーの最大数
				private const int k_color_num_max = 7;//キャラのカラー最大値
				private const int k_color_num_min = 0;								//カラー最小値

				//初期化関数
				public static void Reset()
				{
								game_mode = GameMode.TIME;
								limit_time = k_initial_limit_time;
								remaining = k_initial_remaining;

								for(int i = 0;i < k_player_num_max; i++)
								{
												player_data[i].handicap = 0;
												player_data[i].color = 0;
												player_data[i].player_number = i + 1;
								}
				}
				
				//制限時間の取得
				public static int GetLimitTime()
				{
								return limit_time;
				}
				//残機の取得
				public static int GetRemaining()
				{
								return remaining;
				}

				//制限時間を変更する
				//numには、変更したい量を設定する
				public static void ChangeLimitTime(int num)
				{
								limit_time += num;
								if(limit_time < k_limit_time_min)
								{
												limit_time = k_limit_time_min;
												//ここでそれ以上下げれませんという意思を伝える必要ありSE?
								}
								if (limit_time < k_limit_time_max)
								{
												limit_time = k_limit_time_max;
								}
				}

				//残機数の変更を行う
				public static void ChangeRemaining(int num)
				{
								remaining += num;
								if (remaining < k_remaining_min)
								{
												remaining = k_remaining_min;
												//ここでそれ以上下げれませんという意思を伝える必要ありSE?
								}
								if (remaining < k_remaining_max)
								{
												remaining = k_remaining_max;
								}
				}

				//モード変更　引数で指定したモードに変更する
				public static void ModeChange(int new_mode)
				{
								//列挙の数を超えていないか
								if(new_mode >= (int)GameMode.AMOUNT || new_mode < 0)
								{
												//例外
												Debug.Log("モードの変更はできませんでした");
												return;
								}

								switch ((GameMode)new_mode)
								{
												case GameMode.TIME:
																game_mode = GameMode.TIME;
																//これはモードを変更したときに値を初期化するかどうかで変わる
																limit_time = k_initial_limit_time;

																break;
												case GameMode.STOCK:
																game_mode = GameMode.STOCK;
																remaining = k_initial_remaining;
																break;
								}
				}

				//一フレーム毎に生存を確認する
				private static void FixedUpdate()
				{
								//メニュー時のみ動く
								if (SceneManager.GetActiveScene().name == "Menu")
								{
												//Menu内でのみ処理を行う
												ControllerFetcher.Initialize();

												//コントローラーとの関係を確認
												//for(int i = 0;i < k_player_num_max;i++)
												//{
												//				player_data[i].is_connected = ControllerFetcher.GetIsConnectedNum(i);
												//}
								}
				}
}
