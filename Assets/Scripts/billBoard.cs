using UnityEngine;

using UnityEngine.UI;
using System.Collections;

public class billBoard : MonoBehaviour {
    public Text number;
    int N;
    public bool left;
	// Use this for initialization
	void Start () {
	    if(left)
        {
            N = PlayerPrefs.GetInt("nLeft")-1;
        }
        else
        {

            N = PlayerPrefs.GetInt("nRight")-1;
        }
        number.text = N.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if (left)
        {
            N = PlayerPrefs.GetInt("nLeft")-1;
        }
        else
        {

            N = PlayerPrefs.GetInt("nRight")-1;
        }
        number.text = N.ToString();

    }
}
