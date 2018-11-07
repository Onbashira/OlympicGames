using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockSeting : MonoBehaviour
{
				//全員の残機数を描画する
				public Text stock_tex;
				
				private void Start()
				{
								stock_tex = transform.Find("StockText").GetComponent<Text>();
				}

				private void Update()
				{
								if(stock_tex == null)
								{
												Debug.Log("データが作成できていません");
												return;
								}

								if(ModeSeting.GetRemaining() == 0)
								{
												Debug.Log("ModeSetingを初期化されていません");
								}
								//テキスト変更
								stock_tex.text = "" + ModeSeting.GetRemaining();
				}
}
