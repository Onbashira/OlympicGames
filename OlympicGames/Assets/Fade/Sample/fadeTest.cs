using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeTest : MonoBehaviour {
    [SerializeField]
    Fade fade = null;

    private bool isMainColor = false;
    [SerializeField] Color color1 = Color.white, color2 = Color.white;
    [SerializeField] UnityEngine.UI.Image image = null;

    [SerializeField]
    CanvasGroup group = null;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Fadeout()
    {
        group.blocksRaycasts = false;
        fade.FadeIn(1, () =>
        {
            image.color = (isMainColor) ? color1 : color2;
            isMainColor = !isMainColor;
            fade.FadeOut(1, () => {
                group.blocksRaycasts = true;
            });
        });
    }
}
