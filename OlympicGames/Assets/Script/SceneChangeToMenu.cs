using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeToMenu : MonoBehaviour
{
				// Use this for initialization
				void Start()
				{
								ModeSetting.initialized();
				}

				// Update is called once per frame
				void Update()
				{
								ModeSetting.ConnectedUpdate();

								if (GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.A, (GamepadInput.GamePad.Index)0))
								{
												SceneManager.LoadScene("Menu");
								}
				}
}
