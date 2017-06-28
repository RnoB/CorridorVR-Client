using System;
using UnityEngine;
public class movplayer : MonoBehaviour
{
	public float Speed;
	private void Start()
	{
		this.Speed = PlayerPrefs.GetFloat("Speed");
		//base.transform.rotation = Quaternion.Euler(0f, -Input.compass.magneticHeading, 0f);
	}
	private void Update()
	{
        //transform.Translate(Camera.main.transform.forward * Time.deltaTime * this.Speed);

        Vector3 direction = Camera.main.transform.forward;
        direction.y = 0;
        GetComponent<Rigidbody>().velocity =(direction  * this.Speed);


    }
}
