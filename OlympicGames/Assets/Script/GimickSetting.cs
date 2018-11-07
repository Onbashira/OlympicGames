using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimickSetting : MonoBehaviour
{
				//ギミックをするかどうかの変更
				//ただギミックの状態を変更した時に呼ぶこと
				public void ChangeIsGimick()
				{
								transform.Find("GimickChack").transform.GetComponent<RawImage>().enabled =
												!transform.Find("GimickChack").transform.GetComponent<RawImage>().enabled;
				}

				private void Update()
				{
								if(Input.GetKeyDown(KeyCode.Return))
								{
												ChangeIsGimick();
								}
				}
}
