using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : UnityEngine.UI.Graphic , IFade
{

    [SerializeField,Range(0.0f,1.0f)]
    private float cutoutRange = 1.0f;
    [SerializeField] Texture maskTexture = null;

    public float Range
    {
        get
        {
            return cutoutRange;
        }
        set
        {
            cutoutRange = value;
            UpdateMaskCutout(cutoutRange);
        }
    }


	// Use this for initialization
	protected override void Start () {
        base.Start();
        UpdateMaskTexture(maskTexture);
	}

    void UpdateMaskCutout(float range)
    {
        enabled = true;

        material.SetFloat("_Range", 1.0f - range);

        //if (range <= 0)
        //{
        //    this.enabled = false;
        //}
    }

    public void UpdateMaskTexture(Texture texture)
    {
        material.SetTexture("_MaskTex", texture);
        material.SetColor("_Color", color);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateMaskCutout(Range);
        UpdateMaskTexture(maskTexture);
    }
#endif
}
