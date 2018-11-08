using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;



public class PlayerUICanvas : MonoBehaviour {

    [SerializeField]
    private uint playerStock = 0;

    [SerializeField]
    Image image = null;

    [SerializeField]
    Text text = null;

    [SerializeField]
    RectTransform  rectTransform = null;

    // Use this for initialization
    void Start () {
        image = this.GetComponent<Image>();
        text = this.GetComponent<Text>();
        rectTransform = this.GetComponent<RectTransform>();
    }
    
    // Update is called once per frame
	void Update () {
        text.text = "" + playerStock;
    }

    public void SetTex(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetPos(float x,float y)
    {
        rectTransform.position = new Vector3(x, y, 0.0f);
    }

    public void SetPlayerStock(uint stockNum)
    {
        playerStock = stockNum;
    }

}
