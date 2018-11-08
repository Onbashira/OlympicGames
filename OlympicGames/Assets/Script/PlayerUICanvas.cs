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

    }
    public void Initialized()
    {
        image = this.GetComponentInChildren<Image>();
        text = this.GetComponentInChildren<Text>();
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
        if (playerStock == 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.5f);
        }
    }

}
