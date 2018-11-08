using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockSetting : MonoBehaviour
{
				//全員の残機数を描画する
				private Text stock_tex;
				
				private void Start()
				{
								stock_tex = GetComponent<Text>();
				}
				
				private void Update()
				{
								if(stock_tex == null)
								{
												Debug.Log("データが作成できていません");
												return;
								}

								if(ModeSetting.GetRemaining() == 0)
								{
												Debug.Log("ModeSetingを初期化されていません");
								}
								//テキスト変更
								stock_tex.text = "" + ModeSetting.GetRemaining();
				}
}
