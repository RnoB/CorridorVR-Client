using System;
using UnityEngine;
public class KeyCtrl : MonoBehaviour
{
	public float Speed;
	private void Start()
	{
	}
	private void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
		{
			transform.Translate(Vector3.forward * Time.deltaTime * this.Speed);
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			transform.Translate(Vector3.back * Time.deltaTime * this.Speed);
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
		{
			transform.Translate(Vector3.left * Time.deltaTime * this.Speed);
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			transform.Translate(Vector3.right * Time.deltaTime * this.Speed);
		}
	}
}
