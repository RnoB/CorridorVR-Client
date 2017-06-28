using UnityEngine;
using System.Collections;

public class lightColorTrail : MonoBehaviour
{

    public bool left;
    bool flip;
    float nLeft;
    float nRight;
    float anis;
    float sat;
    float hue;
    Light lightC;
    // Use this for initialization
    void Start()
    {

        lightC = GetComponent<Light>();
        nPick();
        flip = true;
        if(nLeft==0 || nRight==0)
        {
            hue = 0;
            
        }
        if (nLeft > nRight)
        {
            anis = nRight / nLeft;
            hue = 0;
        }
        else
        {
            anis = nLeft / nRight;
            hue = 120f / 360f;
        }
        lightColor();
    }

    // Update is called once per frame
    void Update()
    {
        anisotropy();
        lightColor();


    }

    void lightColor()
    {
        if (flip || nLeft==0 || nRight==0)
        {
            sat = 0;
        }
        else
        {
            sat = 1 - anis;
        }
        HSBColor col2 = new HSBColor(hue, sat, 1, 0);
        HSBColor col1 = HSBColor.FromColor(lightC.color);
        //Color col = (new HSBColor(hue,  sat, 1, 0)).ToColor();
        //Color lerpedColor = Color.Lerp(lightC.color, col, Time.deltaTime);
        Color col = (HSBColor.Lerp(col1, col2, Time.deltaTime)).ToColor();
        lightC.color = col;
        if (flip)
        {
            if (hue == 0)
                hue = 120f / 360f;
            else
                hue = 0;
            flip = !flip;
            col1 = col2;
            col2 = new HSBColor(hue, sat, 1, 0);
            //Color col = (new HSBColor(hue,  sat, 1, 0)).ToColor();
            //Color lerpedColor = Color.Lerp(lightC.color, col, Time.deltaTime);
            col = (HSBColor.Lerp(col1, col2, Time.deltaTime)).ToColor();
            lightC.color = col;

        }
    }
    void anisotropy()
    {
        nPick();
        if (nLeft > nRight)
        {
            anis = nRight / nLeft;
            if (hue > 0)
            {
                flip = !flip;
            }
        }
        else
        {
            anis = nLeft / nRight;
            if (hue == 0)
            {
                flip = !flip;
            }
        }

    }

    void nPick()
    {
        if (left)
        {
            nLeft = (float) PlayerPrefs.GetInt("nLeft");
            nRight = (float) PlayerPrefs.GetInt("nRight");
        }
        else
        {
            nRight = (float) PlayerPrefs.GetInt("nLeft");
            nLeft = (float) PlayerPrefs.GetInt("nRight");
        }
    }
}

