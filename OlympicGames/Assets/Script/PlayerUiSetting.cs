using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiSetting : MonoBehaviour
{
				private ModeSetting.ColorIndex player_color;
				private RawImage player_ui;
				private Text player_color_text;//プレイヤーのカラーのテキスト
				public int player_number = 0;//プレイヤーのナンバー
				
				private void Start()
				{
								player_color = ModeSetting.ColorIndex.GREEN;
								player_ui = transform.parent.GetComponent<RawImage>();
								player_color_text = transform.Find("Texter").GetComponent<Text>();
				}
				
				void Update()
				{
								if (ModeSetting.player_data.Count <= player_number)
								{
												return;
								}
								if (player_color != ModeSetting.player_data[player_number].color)
								{
												player_color = ModeSetting.player_data[player_number].color;
												player_color_text.text =
												ModeSetting.player_data[player_number].color.ToString();
								}
				}
}
