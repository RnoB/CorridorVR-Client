using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DisplaySpectator : MonoBehaviour
{
	public Text displayScore;
	private ManagerScript managerScript;
	private GameObject manager;
	private void Start()
	{
		manager = GameObject.Find("Manager");
		managerScript = manager.GetComponent<ManagerScript>();
	}
	private void Update()
	{
		string text = Time.time.ToString("F2") + "\n";
		for (int i = 0; i < managerScript.nPlayers; i++)
		{
			if (managerScript.listId[i] < 20000)
			{

				text = string.Concat(new string[]{text,
					managerScript.listName[i].ToString(),"  ",
					managerScript.listScore[i].ToString(),"\n"});
			}
		}
		displayScore.text = text;
	}
}
