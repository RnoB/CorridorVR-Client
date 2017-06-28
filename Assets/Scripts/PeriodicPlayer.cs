using UnityEngine;
using System.Collections;

public class PeriodicPlayer : MonoBehaviour {

    public int periodic;
	// Use this for initialization
	void Start () {
        float corridorSize = PlayerPrefs.GetFloat("Ltot");
        transform.Translate(new Vector3(0, 0, periodic * corridorSize));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
