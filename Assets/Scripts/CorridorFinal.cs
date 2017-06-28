using UnityEngine;
using System.Collections;

public class CorridorFinal : MonoBehaviour {

	// Use this for initialization
	void Start () {



        float A1 = 3.14f/4f;
        float Len = 10;
        float Lex = 10;
        float Lco = 20;
        float W = 4;
        float Wbi = 12;
        float H = 3;

        float A = Mathf.PI / 6;
        float Ltot = Len + Lex + Lco + Wbi / Mathf.Tan(A) + Wbi / Mathf.Tan(A1);

        PlayerPrefs.SetFloat("Ltot", Ltot);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
