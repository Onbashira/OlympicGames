using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultSetting : MonoBehaviour
{
				[SerializeField, Tooltip("GameUpdatePreUpdateで要する時間を秒単位で指定")]
				private float maxFadeUpdateTime = 1.0f; //一秒
				[SerializeField]
				private Fade fade = null;

				public GameObject rank_prafab;//順位のプレハブ

				public List<Vector3> rank_ui_pos = new List<Vector3>();

				public Sprite[] tex_rank = new Sprite[4];

				public float time_min = 0;//最低描画時間
				[SerializeField]
				private float time = 0;
				[SerializeField]
				private bool is_scene_change = false;
				
				System.Action ResultState;

				void Start()
				{
								ResultState = FadeInSystem;
								List<int> result_data = GameResultManager.GetPlayerRank();
								for (int i = 0;i < result_data.Count; i++)
								{
												GameObject obj = Instantiate(rank_prafab, Vector3.zero, Quaternion.identity);

												obj.transform.parent = transform.parent;

												obj.GetComponent<RectTransform>().localPosition = rank_ui_pos[i];

												obj.transform.Find("Texter").GetComponent<Text>().text =
																"Player" + i;

												Image img = obj.transform.Find("Rank").GetComponent<Image>();
												img.sprite = tex_rank[i];
								}
				}

				private void Update()
				{
								ResultState();
				}

				void RisultSystem()
				{
								if (GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.A, (GamepadInput.GamePad.Index)0))
								{
												ResultState = FadeOut;
								}
				}

				void FadeInSystem()
				{
								ResultState = Fades;
								fade.FadeIn(maxFadeUpdateTime, () =>
								{
												ResultState = RisultSystem;
								});
				}

				void Fades()
				{

				}
				void FadeOut()
				{
								ResultState = Fades;
								fade.FadeOut(maxFadeUpdateTime, () =>
								{
												SceneManager.LoadScene("Menu");
								});
				}
}
