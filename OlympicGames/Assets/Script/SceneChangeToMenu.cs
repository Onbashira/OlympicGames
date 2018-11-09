﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeToMenu : MonoBehaviour
{
				[SerializeField, Tooltip("GameUpdatePreUpdateで要する時間を秒単位で指定")]
				private float maxFadeUpdateTime = 1.0f; //一秒
				[SerializeField]
				private Fade fade = null;

				bool is_fade_play = false;
				System.Action fade_end;

				// Use this for initialization
				void Start()
				{
								fade_end = FadeOutChack;
				}

				void FadeOutChack()
				{
								if (GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.A, (GamepadInput.GamePad.Index)0))
								{
												fade.FadeOut(maxFadeUpdateTime, () =>
												{
																fade_end = FadeOutEnd;
												});
												fade_end = FadeOut;
								}
				}

				void FadeOut()
				{
								
				}

				void FadeOutEnd()
				{
								SceneManager.LoadScene("Menu");
				}

				// Update is called once per frame
				void Update()
				{
								fade_end();
				}
}
