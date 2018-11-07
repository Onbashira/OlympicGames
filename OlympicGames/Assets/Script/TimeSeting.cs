using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSeting : MonoBehaviour
{
				//制限時間を設定する
				private Text time_tex;

				private void Start()
				{
								time_tex = GetComponent<Text>();
				}

				private void Update()
				{
								//テキスト変更
								time_tex.text = "" + ModeSeting.GetLimitTime();
				}
}
