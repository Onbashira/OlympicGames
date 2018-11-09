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

				private Image p_image;
				public Sprite[] player_ui_sprite;

				private void Start()
				{
								player_color = ModeSetting.ColorIndex.WHITE;
								player_ui = transform.parent.GetComponent<RawImage>();
								player_color_text = transform.Find("Texter").GetComponent<Text>();
								p_image = transform.Find("PlayerFram").GetComponent<Image>();
				}
				
				void Update()
				{
								if (ModeSetting.player_data.Count <= player_number)
								{
												return;
								}
								ModeSetting.ColorIndex color = ModeSetting.player_data[player_number].color;
								if (player_color != color)
								{
												player_color = color;
												player_color_text.text =
												ModeSetting.player_data[player_number].color.ToString();
												p_image.sprite = player_ui_sprite[(int)color]; 
								}
				}
}
