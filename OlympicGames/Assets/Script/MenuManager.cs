using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

//メニュー内でのプレイヤーの行動からなる処理を書いていく
public class MenuManager : MonoBehaviour
{
				//メニュー構成 どのメニューを選択しているか
				//左上　STOCK,　右上　GIMICK,　中心　PLAYER,　右下　SETEND
				enum MenuUI { STOCK, GIMICK, PLAYER, SETEND };

				//例、Player1 ～ 4は各プレイヤーの場所
				//1,2は、ストックの変更の場所へ
				//3,4は、ギミックの変更の場所へ

				//カーソルが現在どのUIを選択しているのか
				private MenuUI[] player_carsor_ui = new MenuUI[ModeSetting.k_player_num_max];

				//メニュ―で設定する機能
				private StockSetting stock_set;//ストックの設定を行う
				private GimickSetting gimick_set; //ギミックを使用するかどうか

				//入力を判断するための差分
				private GamepadInput.GamepadState gamepad_state;
				private GamepadInput.GamepadState gamepad_state_old;
				
				//ルールの決定が終わりました
				public bool is_menu_end;

				System.Action input_updater;
				System.Action player_updater;

				
				//初期化
				private void Initialized()
				{
								input_updater = GamepadStateUpdate;
								player_updater = PlayerUpdate;
								is_menu_end = false;
				}

				private void Start()
				{
								stock_set = transform.Find("Stock").GetComponent<StockSetting>();
								gimick_set = transform.Find("Gimick").GetComponent<GimickSetting>();
								
								//初期化
								for (int i = 0; i < ModeSetting.k_player_num_max; i++)
								{
												player_carsor_ui[i] = MenuUI.PLAYER;
								}
								Initialized();
				}

				void Update()
				{
								if(!is_menu_end)
								{
												//セレクトの処理がまだ終わっていません
												input_updater();
												player_updater();
								}
								else
								{
												//セレクトでの処理が終わりました
												//フェードインが終わり次第シーンを変えます

								}
				}

				//プレイヤーの行動
				private void PlayerUpdate()
				{
								for (int i = 0; i < ModeSetting.k_player_num_max; i++)
								{
												if (!ModeSetting.player_data[i].is_connected)
												{
																//コントローラーとの接続が絶たれています
																//Debug.Log("指定されたコントローラーの接続が確認できません");
																continue;
												}

												//接続が確認できました
												if (player_carsor_ui[i] == MenuUI.STOCK)
												{
																//ストックの関数
																StockUiBehavior(i);
																continue;
												}
												else if (player_carsor_ui[i] == MenuUI.GIMICK)
												{
																//ギミックの関数
																GimickUiBehavior(i);
																continue;
												}
												else if (player_carsor_ui[i] == MenuUI.PLAYER)
												{
																//プレイヤーの関数
																PlayerUiBehavior(i);
																continue;
												}
												else
												{
																//対戦へ移行します
																SetEndUiBehavior(i);
																continue;
												}
								}
				}
				//ストックUI上での処理
				private void StockUiBehavior(int player_num)
				{
								if (gamepad_state.Right && !gamepad_state_old.Right)
								{
												//ストックからギミックの選択場所へ移動
												player_carsor_ui[player_num] = MenuUI.GIMICK;
												//ここでカーソルの移動処理を
												return;
								}
								else if (gamepad_state.Down && !gamepad_state_old.Down)
								{
												//ストックからプレイヤーの色選択へ移動
												player_carsor_ui[player_num] = MenuUI.PLAYER;
												//ここでカーソルの移動処理を
												return;
								}
								else if (gamepad_state.LeftShoulder && !gamepad_state_old.LeftShoulder)
								{
												//ストックの減算
												ModeSetting.ChangeRemaining(-1);
								}
								else if (gamepad_state.RightShoulder && !gamepad_state_old.RightShoulder)
								{
												//ストックの加算
												ModeSetting.ChangeRemaining(1);
								}
				}

				//ギミックUI上での処理
				private void GimickUiBehavior(int player_num)
				{
								if (gamepad_state.Left && !gamepad_state_old.Left)
								{
												//ストックからギミックの選択場所へ移動
												player_carsor_ui[player_num] = MenuUI.GIMICK;
												//ここでカーソルの移動処理を
												return;
								}
								else if (gamepad_state.Down && !gamepad_state_old.Down)
								{
												//ストックからプレイヤーの色選択へ移動
												player_carsor_ui[player_num] = MenuUI.PLAYER;
												//ここでカーソルの移動処理を
												return;
								}

								//ここどうしましょうか、入力

								else if (gamepad_state.LeftShoulder && !gamepad_state_old.LeftShoulder)
								{
												//ストックの減算
												ModeSetting.ChangeIsGimick();
								}
								else if (gamepad_state.RightShoulder && !gamepad_state_old.RightShoulder)
								{
												//ストックの加算
												ModeSetting.ChangeIsGimick();
								}
				}

				private void PlayerUiBehavior(int player_num)
				{
								if (gamepad_state.Up && !gamepad_state_old.Up)
								{
												if(player_num < 2)
												{
																//画面左側のプレイヤーはストックの選択場所へ移動
																player_carsor_ui[player_num] = MenuUI.STOCK;
												}
												else
												{
																//画面右側のプレイヤーはギミックの選択場所へ移動
																player_carsor_ui[player_num] = MenuUI.GIMICK;
												}
												//ここでカーソルの移動処理を
								}
								else if (gamepad_state.Down && !gamepad_state_old.Down)
								{
												//ストックからプレイヤーの色選択へ移動
												player_carsor_ui[player_num] = MenuUI.SETEND;
												//ここでカーソルの移動処理を
								}
								
								//LRでのカラー変換
								else if (gamepad_state.LeftShoulder && !gamepad_state_old.LeftShoulder)
								{
												ModeSetting.ChangePlayerColor(player_num,-1);
								}
								else if (gamepad_state.RightShoulder && !gamepad_state_old.RightShoulder)
								{
												ModeSetting.ChangePlayerColor(player_num, 1);
								}
								return;
				}

				//セレクト終了UI上での処理
				private void SetEndUiBehavior(int player_num)
				{
								if (gamepad_state.Up && !gamepad_state_old.Up)
								{
												player_carsor_ui[player_num] = MenuUI.PLAYER;
								}

								if (gamepad_state.A && !gamepad_state_old.A)
								{
												is_menu_end = true;
								}
				}

				//コントローラーの入力が行われたタイミングをとるために
				//比べるデータの作成
				void GamepadStateUpdate()
				{
								for(int i= 0;i < ModeSetting.k_player_num_max;i++)
								{
												gamepad_state_old = gamepad_state;
												gamepad_state = GamePad.GetState((GamePad.Index)i);
								}
				}
}
