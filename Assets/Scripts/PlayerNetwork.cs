using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerNetwork{
	public int clientId;
	public double X = new double ();
	public double Y = new double ();
	public double Z = new double ();
	public List<Vector3> posPlayer = new List<Vector3> ();
	public float Speed;

	public Vector3 target;
	public Vector3 position;
    public Vector3 positionR;
    public float distance;

    public PlayerNetwork(int Id,double X,double Y,double Z,float Speed) 
	{
		this.Speed = Speed;
		clientId = Id;
		this.X=X;
		this.Y=Y;
		this.Z=Z;
		posPlayer.Add (new Vector3 ((float)X, (float)Y, (float)Z));
		position = new Vector3 ((float)X, (float)Y, (float)Z);
        positionR = new Vector3((float)X, (float)Y, (float)Z);
        target = new Vector3 ((float)X, (float)Y, (float)Z);
	}
	public void newPos(double X,double Y,double Z) 
	{


		posPlayer.Add (new Vector3 ((float)X, (float)Y, (float)Z));
		if (posPlayer.Count > 3) {

			posPlayer.RemoveAt(0);
		}


	}
	public void positionUpdate(Vector3 positionObject) 
	{

		Vector3 direction = posPlayer.Last () - posPlayer.First ();
		direction = posPlayer.Last()+5*direction.normalized*Time.deltaTime*Speed;
		target = (direction - positionR).normalized;
        positionR += (target * Time.deltaTime * Speed);
        position = positionObject;

        //Debug.Log ("client Id" + clientId);
        //Debug.Log ("target" + target);
        //Debug.Log ("position" + positionR);
        //Debug.Log ("Last position received" + posPlayer.Last());
        //Debug.Log ("position difference" + (position-posPlayer.Last ()));

	}
	// Use this for initialization

	
	// Update is called once per frame

}
