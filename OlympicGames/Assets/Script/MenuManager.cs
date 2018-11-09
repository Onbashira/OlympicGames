using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using UnityEngine.SceneManagement;

//メニュー内でのプレイヤーの行動からなる処理を書いていく
//2
//4
//1
//のメニュー状態での処理
public class MenuManager : MonoBehaviour
{
				//メニュー構成 どのメニューを選択しているか
				//左上　STOCK,　右上　GIMICK,　中心　PLAYER,　右下　SETEND
				enum MenuUI { STOCK, PLAYER1, PLAYER2, PLAYER3, PLAYER4, SETEND };

				struct PlayerCarsor
				{
								public MenuUI menu_ui;
								public GameObject carsor_prafab;
				}

				//例、Player1 ～ 4は各プレイヤーの場所
				//1,2は、ストックの変更の場所へ
				//3,4は、ギミックの変更の場所へ
				//カーソルが現在どのUIを選択しているのか
				private PlayerCarsor player_carsor = new PlayerCarsor();

				public RectTransform stock_ui_data;
				public RectTransform select_ui_data;
				public List<RectTransform> player_ui_data = new List<RectTransform>();

				//メニュ―で設定する機能
				private StockSetting stock_set;//ストックの設定を行う
				private GimickSetting gimick_set; //ギミックを使用するかどうか

				//入力を判断するための差分
				private GamepadInput.GamepadState gamepad_state = new GamepadInput.GamepadState();
				private GamepadInput.GamepadState gamepad_state_old = new GamepadInput.GamepadState();

				//ルールの決定が終わりました
				public bool is_menu_end;

				//初期化
				public void Initialized()
				{
								is_menu_end = false;
								CleateCarsor();
				}

				private void Start()
				{
								stock_set = transform.Find("Stock").GetComponent<StockSetting>();
								
								Initialized();
								for (int i = 0; i < ModeSetting.k_player_num_max; i++)
								{
												gamepad_state = gamepad_state_old = GamePad.GetState((GamePad.Index)0);
								}
				}

				void Update()
				{
								ModeSetting.ConnectedUpdate();
								if (!is_menu_end)
								{
												//セレクトの処理がまだ終わっていません
												GamepadStateUpdate();
												PlayerUpdate();
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
								if (ModeSetting.player_data.Count == 0)
								{
												//コントローラーとの接続が絶たれています
												Debug.Log("1Pの接続が確認できません");
								}

								PlayerCarsor p_carsor = player_carsor;

								//接続が確認できました
								if (p_carsor.menu_ui == MenuUI.STOCK)
								{
												//ストックの関数
												StockUiBehavior();
								}
								else if (p_carsor.menu_ui == MenuUI.PLAYER1 ||
												p_carsor.menu_ui == MenuUI.PLAYER2 ||
												p_carsor.menu_ui == MenuUI.PLAYER3 ||
												p_carsor.menu_ui == MenuUI.PLAYER4)
								{
												//プレイヤーの関数
												PlayerUiBehavior();
								}
								else
								{
												//対戦へ移行します
												SetEndUiBehavior();
								}
				}
				//ストックUI上での処理
				private void StockUiBehavior()
				{
								if (gamepad_state.LeftShoulder && !gamepad_state_old.LeftShoulder)
								{
												//ストックの減算
												ModeSetting.ChangeRemaining(-1);
												return;
								}
								else if (gamepad_state.RightShoulder && !gamepad_state_old.RightShoulder)
								{
												//ストックの加算
												ModeSetting.ChangeRemaining(1);
												return;
								}
								if (gamepad_state.Down && !gamepad_state_old.Down)
								{
												//ストックからプレイヤーの色選択へ移動
												player_carsor.menu_ui = MenuUI.PLAYER1;
								}
								//ここでカーソルの移動処理を
								SetCarsorPos();
				}

				private void PlayerUiBehavior()
				{
								if(ModeSetting.player_data.Count == 0)
								{
												//なにも接続されていない
												Debug.Log("１Pの接続を確認できません");
												return;
								}

								//LRでのカラー変換
								if (gamepad_state.LeftShoulder && !gamepad_state_old.LeftShoulder)
								{
												ModeSetting.ChangePlayerColor((int)player_carsor.menu_ui - 1, -1);
								}
								else if (gamepad_state.RightShoulder && !gamepad_state_old.RightShoulder)
								{
												ModeSetting.ChangePlayerColor((int)player_carsor.menu_ui - 1, 1);
								}

								//プレイヤーのカーソル移動をする
								//上
								if (gamepad_state.Up && !gamepad_state_old.Up)
								{
												if (player_carsor.menu_ui == MenuUI.PLAYER1 ||
																player_carsor.menu_ui == MenuUI.PLAYER2)
												{
																//画面左側のプレイヤーはストックの選択場所へ移動
																player_carsor.menu_ui = MenuUI.STOCK;
												}
												else
												{
																player_carsor.menu_ui -= 2;
												}
								}
								//下
								else if (gamepad_state.Down && !gamepad_state_old.Down)
								{
												if (player_carsor.menu_ui == MenuUI.PLAYER1 ||
																player_carsor.menu_ui == MenuUI.PLAYER2)
												{
																player_carsor.menu_ui += 2;
												}
												else
												{
																//ストックからプレイヤーの色選択へ移動
																player_carsor.menu_ui = MenuUI.SETEND;
												}
								}
								//右
								else if (gamepad_state.Left && !gamepad_state_old.Left)
								{
												if (player_carsor.menu_ui == MenuUI.PLAYER2 ||
												player_carsor.menu_ui == MenuUI.PLAYER4)
												{
																player_carsor.menu_ui -= 1;
												}
								}
								else if (gamepad_state.Right && !gamepad_state_old.Right)
								{
												if (player_carsor.menu_ui == MenuUI.PLAYER1 ||
																player_carsor.menu_ui == MenuUI.PLAYER3)
												{
																player_carsor.menu_ui += 1;
												}
								}
								SetCarsorPos();
				}

				//セレクト終了UI上での処理
				private void SetEndUiBehavior()
				{
								if (gamepad_state.A && !gamepad_state_old.A)
								{
												//誰かがスタートボタンを押しました
												is_menu_end = true;
												return;
								}
								if (gamepad_state.Up && !gamepad_state_old.Up)
								{
												player_carsor.menu_ui = MenuUI.PLAYER4;
								}
								else
								{
												return;
								}
								SetCarsorPos();
				}

				private void SetCarsorPos()
				{
								RectTransform raw;
								if(player_carsor.menu_ui == MenuUI.STOCK)
								{
												raw = stock_ui_data;
								}
								else if(player_carsor.menu_ui == MenuUI.PLAYER1 ||
																player_carsor.menu_ui == MenuUI.PLAYER2 ||
																player_carsor.menu_ui == MenuUI.PLAYER3 ||
																player_carsor.menu_ui == MenuUI.PLAYER4)
								{
												raw = player_ui_data[(int)player_carsor.menu_ui - 1];
								}
								else
								{
												raw = select_ui_data;
								}
								player_carsor.carsor_prafab.GetComponent<RectTransform>().localPosition =
												raw.localPosition;

								Vector2 size = raw.sizeDelta;

								player_carsor.carsor_prafab.transform.Find("LeftUpCarsor").GetComponent<RectTransform>().localPosition =
												new Vector3
												(
																-size.x / 6,
																size.y / 6,
																0.0f
												);
								player_carsor.carsor_prafab.transform.Find("RightDownCarsor").GetComponent<RectTransform>().localPosition =
												new Vector3
												(
																size.x / 6,
																-size.y / 6,
																0.0f
												);
				}

				public void CleateCarsor()
				{
								player_carsor.carsor_prafab = Instantiate((GameObject)Resources.Load("PlayerCarsor"), Vector3.zero, Quaternion.identity);
								player_carsor.carsor_prafab.transform.parent = transform;
								player_carsor.menu_ui = MenuUI.PLAYER1;
								SetCarsorPos();//初期座標に調整
				}
				
				//コントローラーの入力が行われたタイミングをとるために
				//比べるデータの作成
				void GamepadStateUpdate()
				{
								//使われているコントローラーの情報のみ更新
								for (int i = 0; i < ModeSetting.player_data.Count; i++)
								{
												gamepad_state_old = gamepad_state;
												//リストに格納されているもの順に調べる

												///ここでコントローラーの番号を+1すること
												gamepad_state =	GamePad.GetState((GamePad.Index)ModeSetting.player_data[i].player_number + 1);
												break;
								}
				}
}
