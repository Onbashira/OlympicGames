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
    public List<RectTransform>  GetPlayerUIPos()
    {
        return playerUIPos;
    }
    public List<PlayerUICanvas> GetCanvases()
    {
        return playerUICanvases;
    }

    public void CreatPlayerCanvas(PlayerController player)
    {
        var uc = Instantiate((GameObject)Resources.Load("playerReserveUI"),this.transform).GetComponent<PlayerUICanvas>();
        uc.Initialized();
        uc.SetPos(playerUIPos[(int)player.GetPlayerNO()-1].position.x, playerUIPos[(int)player.GetPlayerNO()-1].position.y);
        uc.SetPlayerStock(player.GetPlayerStock());
        uc.SetTex(player.GetComponent<SpriteRenderer>().sprite);
        playerUICanvases.Add(uc);
    }

    public void CanvasUpdate(PlayerController player)
    {
        playerUICanvases[(int)player.GetPlayerNO()-1].SetPlayerStock(player.GetPlayerStock());
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void LateUpdate()
    {
        
    }
}
