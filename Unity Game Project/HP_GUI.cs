/*
 * Written by Sam Arutyunyan, 2014 for 'Whiskers of Death Game' project. 
 * displays the healthbar UI. Uses GUITexture objects within the scene. 
*/
using UnityEngine;
using System.Collections;

public class HP_GUI : MonoBehaviour
{
    public GUITexture hurtTex;
    public GUITexture hpTex;
    public GUITexture hpBarAnchor;
    public float currentHP = 100;
    float maxHP = 100;
    float hpPercent = 0;
    Color hpColor = Color.yellow;
    float alphaMin = .15f;
    float alphaMax = .5f;
    float hurtAlpha = .15f; // alternates between min and max to create blinking
    bool forward = true;
    float hpVel = 0;        // ref for SmoothDamp()

    void Update()
    { 
        if (currentHP < 0) currentHP = 0;
        hpPercent = currentHP / maxHP;

        //draw the background:
        if (hpPercent < .3f)
        {
            hurtTex.gameObject.SetActive(true);
            hurtTex.color = new Color(.5f, .5f, .5f, hurtAlpha);

            if (hurtAlpha <= alphaMin && !forward)
            {
                //go one way
                forward = true;
            }
            else if (hurtAlpha >= alphaMax && forward)
            {
                forward = false;
            }

            if (forward)
            {
                //hurtAlpha = Mathf.Lerp(hurtAlpha, 1, Time.deltaTime * 3);
                hurtAlpha = Mathf.SmoothDamp(hurtAlpha, alphaMax, ref hpVel, .2f);
                if (hurtAlpha > alphaMax - .05) hurtAlpha = alphaMax;
            }
            else
            {
                //hurtAlpha = Mathf.Lerp(hurtAlpha, .3f, Time.deltaTime * 3);
                hurtAlpha = Mathf.SmoothDamp(hurtAlpha, alphaMin, ref hpVel, .2f);
                if (hurtAlpha < alphaMin + .05) hurtAlpha = alphaMin;
            }
        }
        else
        {
            hurtTex.gameObject.SetActive(false);
        }

        hpTex.color = new Color(.5f, hpPercent * .5f, hpPercent * .5f, .5f);
        hpBarAnchor.transform.localScale = new Vector3(hpPercent, 1, 1);
    }
}