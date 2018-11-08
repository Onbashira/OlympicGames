using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSetting : MonoBehaviour
{
				private GameObject rank_prafab = new GameObject();//順位のプレハブ

				public List<Vector3> rank_ui_pos = new List<Vector3>();

				void Start()
				{
								rank_prafab = (GameObject)Resources.Load("ResultRank");

								List<int> result_data = GameResultManager.GetPlayerRank();
								for (int i = 0;i < result_data.Count; i++)
								{
												GameObject obj = Instantiate(rank_prafab, Vector3.zero, Quaternion.identity);
												obj.transform.position = rank_ui_pos[i];
								}
				}
				


}
