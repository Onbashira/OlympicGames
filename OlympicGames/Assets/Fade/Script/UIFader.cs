using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(Mask))]
public class UIFader : MonoBehaviour,IFade
{
    [SerializeField, Range(0, 1)]
    private float cutoutRange;
    [SerializeField] Material mat = null;
    [SerializeField] RenderTexture rt = null;
    [SerializeField] Texture texture = null;

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

    private void UpdateMaskCutout(float range)
    {
        mat.SetFloat("_Range", range);

        UnityEngine.Graphics.Blit(texture, rt, mat);

        var mask = GetComponent<Mask>();
        mask.enabled = false;
        mask.enabled = true;
    }
}
