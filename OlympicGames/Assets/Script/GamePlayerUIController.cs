using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerUIController : MonoBehaviour {

    [SerializeField]
    List<RectTransform> playerUIPos = new List<RectTransform>();
    [SerializeField]
    List<PlayerUICanvas> playerUICanvases = new List<PlayerUICanvas>();

	// Use this for initialization
	void Start ()
    {
		
	}

    void Initialize()
    {

    }
    
    public void CreatPlayerCanvas(PlayerController player)
    {
        PlayerUICanvas pc = new PlayerUICanvas();
        pc.SetPos(playerUIPos[(int)player.GetPlayerNO()].position.x, playerUIPos[(int)player.GetPlayerNO()].position.y);
        pc.SetPlayerStock(player.GetPlayerStock());
        pc.SetTex(player.GetComponent<SpriteRenderer>().sprite);
        playerUICanvases.Add(pc);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void LateUpdate()
    {
        
    }
}
