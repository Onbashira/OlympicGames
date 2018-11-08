using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GimickSetting : MonoBehaviour
{
				private RawImage gimick_image;

				private void Start()
				{
								gimick_image = GetComponent<RawImage>();
				}

				//ギミックをするかどうかの変更
				//ただギミックの状態を変更した時に呼ぶこと
				public void ChangeIsGimick()
				{
								gimick_image.enabled = !gimick_image.enabled;
				}
}
