using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using UnityEngine.SceneManagement;

//メニュー内でのプレイヤーの行動からなる処理を書いていく
public class MenuManager : MonoBehaviour
{
				//メニュー構成 どのメニューを選択しているか
				//左上　STOCK,　右上　GIMICK,　中心　PLAYER,　右下　SETEND
				enum MenuUI { STOCK, GIMICK, PLAYER, SETEND };

				struct PlayerCarsor
				{
								public MenuUI menu_ui;
								public GameObject carsor_prafab;
				}

				//例、Player1 ～ 4は各プレイヤーの場所
				//1,2は、ストックの変更の場所へ
				//3,4は、ギミックの変更の場所へ
				//カーソルが現在どのUIを選択しているのか
				private List<PlayerCarsor> player_carsor = new List<PlayerCarsor>();
				
				public List<GameObject> ui_data = new List<GameObject>();
				
				//メニュ―で設定する機能
				private StockSetting stock_set;//ストックの設定を行う
				private GimickSetting gimick_set; //ギミックを使用するかどうか

				//入力を判断するための差分
				private GamepadInput.GamepadState[] gamepad_state = new GamepadInput.GamepadState[4];
				private GamepadInput.GamepadState[] gamepad_state_old = new GamepadInput.GamepadState[4];
				
				//ルールの決定が終わりました
				public bool is_menu_end;

				System.Action input_updater;
				System.Action player_updater;
				
				//初期化
				public void Initialized()
				{
								input_updater = GamepadStateUpdate;
								player_updater = PlayerUpdate;
								is_menu_end = false;
				}

				private void Start()
				{
								stock_set = transform.Find("Stock").GetComponent<StockSetting>();
								gimick_set = transform.Find("Gimick").GetComponent<GimickSetting>();
								

								Initialized();
								for (int i = 0; i < ModeSetting.k_player_num_max; i++)
								{
												gamepad_state[i] = gamepad_state_old[i] = GamePad.GetState((GamePad.Index)0);
								}
				}

				void Update()
				{
								ModeSetting.ChackPlayerConnected();
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
												SceneManager.LoadScene("GameMain");
												return;
								}
				}

				//プレイヤーの行動
				private void PlayerUpdate()
				{
								for (int i = 0; i < ModeSetting.player_data.Count; i++)
								{
												if (!ModeSetting.player_data[i].is_connected)
												{
																//コントローラーとの接続が絶たれています
																Debug.Log(i + "Pの接続が確認できません");
																continue;
												}

												if(ModeSetting.is_connect_now[i] == ModeSetting.ConnectState.ON)
												{
																CleateCarsor();
																continue;
												}
												else if(ModeSetting.is_connect_now[i] == ModeSetting.ConnectState.OFF)
												{
																CarsorDeleate(i);
																continue;
												}

												//テスト処理
												if(gamepad_state[i].B && !gamepad_state_old[i].B)
												{
																//セレクト画面終わり
																is_menu_end = true;
																break;
												}

												PlayerCarsor p_carsor = player_carsor[i];

												//接続が確認できました
												if (p_carsor.menu_ui == MenuUI.STOCK)
												{
																//ストックの関数
																StockUiBehavior(i);
																continue;
												}
												else if (p_carsor.menu_ui == MenuUI.GIMICK)
												{
																//ギミックの関数
																GimickUiBehavior(i);
																continue;
												}
												else if (p_carsor.menu_ui == MenuUI.PLAYER)
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
								PlayerCarsor p_carsor = player_carsor[player_num];
								if (gamepad_state[player_num].Right && !gamepad_state_old[player_num].Right)
								{
												//ストックからギミックの選択場所へ移動
												p_carsor.menu_ui = MenuUI.GIMICK;
												//ここでカーソルの移動処理を
												SetCarsorPos(player_num);
												return;
								}
								else if (gamepad_state[player_num].Down && !gamepad_state_old[player_num].Down)
								{
												//ストックからプレイヤーの色選択へ移動
												p_carsor.menu_ui = MenuUI.PLAYER;
												//ここでカーソルの移動処理を
												SetCarsorPos(player_num);
												return;
								}
								else if (gamepad_state[player_num].LeftShoulder && !gamepad_state_old[player_num].LeftShoulder)
								{
												//ストックの減算
												ModeSetting.ChangeRemaining(-1);
								}
								else if (gamepad_state[player_num].RightShoulder && !gamepad_state_old[player_num].RightShoulder)
								{
												//ストックの加算
												ModeSetting.ChangeRemaining(1);
								}
				}

				//ギミックUI上での処理
				private void GimickUiBehavior(int player_num)
				{
								PlayerCarsor p_carsor = player_carsor[player_num];
								if (gamepad_state[player_num].Left && !gamepad_state_old[player_num].Left)
								{
												//ストックからギミックの選択場所へ移動
												p_carsor.menu_ui = MenuUI.GIMICK;
												//ここでカーソルの移動処理を
												SetCarsorPos(player_num);
								}
								else if (gamepad_state[player_num].Down && !gamepad_state_old[player_num].Down)
								{
												//ストックからプレイヤーの色選択へ移動
												p_carsor.menu_ui = MenuUI.PLAYER;
												//ここでカーソルの移動処理を
												SetCarsorPos(player_num);
								}

								//ギミックの変更
								else if (gamepad_state[player_num].LeftShoulder && !gamepad_state_old[player_num].LeftShoulder)
								{
												//ストックの減算
												ModeSetting.ChangeIsGimick();
								}
								else if (gamepad_state[player_num].RightShoulder && !gamepad_state_old[player_num].RightShoulder)
								{
												//ストックの加算
												ModeSetting.ChangeIsGimick();
								}
								player_carsor[player_num] = p_carsor;
				}

				private void PlayerUiBehavior(int player_num)
				{
								PlayerCarsor p_carsor = player_carsor[player_num];
								if (gamepad_state[player_num].Up && !gamepad_state_old[player_num].Up)
								{
												if(player_num < 2)
												{
																//画面左側のプレイヤーはストックの選択場所へ移動
																p_carsor.menu_ui = MenuUI.STOCK;
												}
												else
												{
																//画面右側のプレイヤーはギミックの選択場所へ移動
																p_carsor.menu_ui = MenuUI.GIMICK;
												}


												//更新しろやーーーーーーーー

												//ここでカーソルの移動処理を
												SetCarsorPos(player_num);
								}
								else if (gamepad_state[player_num].Down && !gamepad_state_old[player_num].Down)
								{
												//ストックからプレイヤーの色選択へ移動
												p_carsor.menu_ui = MenuUI.SETEND;
												//ここでカーソルの移動処理を
												SetCarsorPos(player_num);
								}
								//LRでのカラー変換
								else if (gamepad_state[player_num].LeftShoulder && !gamepad_state_old[player_num].LeftShoulder)
								{
												ModeSetting.ChangePlayerColor(player_num,-1);
								}
								else if (gamepad_state[player_num].RightShoulder && !gamepad_state_old[player_num].RightShoulder)
								{
												ModeSetting.ChangePlayerColor(player_num, 1);
								}
								player_carsor[player_num] = p_carsor;

				}

				//セレクト終了UI上での処理
				private void SetEndUiBehavior(int player_num)
				{
								PlayerCarsor p_carsor = player_carsor[player_num];
								if (gamepad_state[player_num].Up && !gamepad_state_old[player_num].Up)
								{
												p_carsor.menu_ui = MenuUI.PLAYER;
												SetCarsorPos(player_num);
								}
								if (gamepad_state[player_num].A && !gamepad_state_old[player_num].A)
								{
												is_menu_end = true;
								}
								player_carsor[player_num] = p_carsor;
				}

				private void SetCarsorPos(int player_num)
				{
								PlayerCarsor p_carsor = player_carsor[player_num];

								p_carsor.carsor_prafab.GetComponent<RectTransform>().localPosition = 
												ui_data[(int)p_carsor.menu_ui].GetComponent<RectTransform>().localPosition;

								Vector2 size = ui_data[(int)p_carsor.menu_ui].GetComponent<RectTransform>().sizeDelta;
								
								p_carsor.carsor_prafab.transform.Find("LeftUpCarsor").GetComponent<RectTransform>().localPosition = 
												new Vector3
												(
																-size.x / 2,
																size.y / 2,
																0.0f
												);
								p_carsor.carsor_prafab.transform.Find("RightDownCarsor").GetComponent<RectTransform>().localPosition =
												new Vector3
												(
																size.x / 2,
																-size.y / 2, 
																0.0f
												);
								player_carsor[player_num] = p_carsor;
				}

				public void CleateCarsor()
				{
								PlayerCarsor p_carsor;
								p_carsor.carsor_prafab = Instantiate((GameObject)Resources.Load("PlayerCarsor"), Vector3.zero, Quaternion.identity);
								p_carsor.carsor_prafab.transform.parent = transform;
								p_carsor.menu_ui = MenuUI.PLAYER;

								player_carsor.Add(p_carsor);
								SetCarsorPos(player_carsor.Count - 1);//初期座標に調整
				}

				public void CarsorDeleate(int deleate_player)
				{
								if(player_carsor.Count <= deleate_player)
								{
												return;
								}
								player_carsor.RemoveAt(deleate_player);
				}

				//コントローラーの入力が行われたタイミングをとるために
				//比べるデータの作成
				void GamepadStateUpdate()
				{
								//使われているコントローラーの情報のみ更新
								for(int i= 0;i < ModeSetting.player_data.Count;i++)
								{
												gamepad_state_old[i] = gamepad_state[i];
												//リストに格納されているもの順に調べる
												gamepad_state[ModeSetting.player_data[i].player_number] =
																GamePad.GetState((GamePad.Index)ModeSetting.player_data[i].player_number);
								}
				}
}
