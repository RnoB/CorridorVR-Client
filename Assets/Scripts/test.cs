using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class test : MonoBehaviour {

    public GameObject corridor;
    public GameObject player;
    public GameObject perPlayer;

    GameObject player2;
    GameObject player3;
    float corridorSize;



    //infinite loop
    private bool startLoop;
    public static List<Vector3> positionGhost = new List<Vector3>();
    private List<GameObject> ghosts = new List<GameObject>();
    private List<bool> ghostLap = new List<bool>();
    private bool playerLap;
    private int N;
    private int T;
    private int Tc;
    private string positionPath;
    private string paramPath;
    private int nLeft;
    private int nRight;
    FileStream fs;



    // Use this for initialization
    void Start () {
        nLeft = 1;
        nRight = 1;
        PlayerPrefs.SetFloat("Aco",3.14f/4f);
        PlayerPrefs.SetFloat("Len",10);
        PlayerPrefs.SetFloat("Lex",10);
        PlayerPrefs.SetFloat("Lco",20);
        PlayerPrefs.SetFloat("Wco",4);
        PlayerPrefs.SetFloat("Wbi",12);
        PlayerPrefs.SetFloat("Hco",3);

        Instantiate(corridor, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        corridorSize = PlayerPrefs.GetFloat("Ltot");

        PlayerPrefs.SetFloat("Speed",2f);
        player2 = (GameObject)UnityEngine.Object.Instantiate(player, new Vector3(0,2, 0), new Quaternion(0, 0, 0, 0));
        //player3 = (GameObject)UnityEngine.Object.Instantiate(perPlayer, new Vector3(0, 0, -5), new Quaternion(0, 0, 0, 0));
        //int nLeft = (int)((1 + Mathf.Sin(Time.time)) * 10);
        //int nRight = (int)((1 + Mathf.Cos(Time.time)) * 10);
        PlayerPrefs.SetInt("nLeft", 1);
        PlayerPrefs.SetInt("nRight", 1);
        Debug.Log(corridorSize);



        //infinite loop
        startLoop = false;
        positionPath = "position";
        paramPath = "param";
        startLoop = !startLoop;
        playerLap = true;

        Tc = 200;
        N = 1;

    }

    // Update is called once per frame
    void Update () {
	    if(player2.transform.position.z>0)
        {
            player2.transform.Translate(new Vector3(0,0,-corridorSize));
        }

        //int nLeft = (int)((1 + Mathf.Sin(Time.time)) * 10);
        //int nRight = (int)((1 + Mathf.Cos(Time.time)) * 10);
        corridorChoiceDetection();
        PlayerPrefs.SetInt("nLeft", nLeft);
        PlayerPrefs.SetInt("nRight", nRight);
    }

    void FixedUpdate()
    {
        if (startLoop)
        {
            T++;
            positionGhost.Add(player2.transform.position);
            if (ghosts.Count > 0)
            {
                for (int k = 0; k < ghosts.Count; k++)
                {
                    ghosts[k].transform.position = new Vector3(positionGhost[T - (k+1) * Tc].x,0, positionGhost[T - (k + 1) * Tc].z);
                }
            }
            if (T % Tc == 0 && ghosts.Count < N)
            {
#if UNITY_ANDROID
                ghosts.Add((GameObject)
                         Instantiate(perPlayer, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)));
#else
				ghosts.Add ((GameObject)
			            Instantiate(perPlayer, new Vector3(0,0,0), new Quaternion(0,0,0,0)));
#endif
                ghostLap.Add(true);
            }
        }
        string appendText = Time.time.ToString() + "\t" + transform.position.x.ToString() + "\t" + Environment.NewLine;
        File.AppendAllText(positionPath, appendText);
    }


    void corridorChoiceDetection()
    {

        float Lex=PlayerPrefs.GetFloat("Lex");
        float A1 = PlayerPrefs.GetFloat("Aco");
        float Len = PlayerPrefs.GetFloat("Len");
        float Lco = PlayerPrefs.GetFloat("Lco");
        float Wco = PlayerPrefs.GetFloat("Wco");
        float Wbi = PlayerPrefs.GetFloat("Wbi");
        Debug.Log(playerLap);
        if (playerLap == false)
        {
            if (player2.transform.position.z > -Lex)
                playerLap = !playerLap;
        }
        else if (player2.transform.position.z < -(Lex + Wbi / Mathf.Tan(A1))
             && player2.transform.position.z > -(Lex + Lco + Wbi / Mathf.Tan(A1)))
        {
            if (player2.transform.position.x > Wco)
            {
                nRight++;
            }
            else if (player2.transform.position.x < -Wco)
            {
                nLeft++;
            }
            playerLap = !playerLap;
        }
        for (int k=0;k<ghosts.Count;k++)
        {
            if (ghostLap[k] == false)
            {
                if (ghosts[k].transform.position.z > -Lex)
                    ghostLap[k] = !ghostLap[k];
            }
            else if (ghosts[k].transform.position.z < -(Lex + Wbi / Mathf.Tan(A1))
                 && ghosts[k].transform.position.z > -(Lex + Lco + Wbi / Mathf.Tan(A1)))
            {
                if (ghosts[k].transform.position.x > Wco)
                {
                    nRight++;
                }
                else if (ghosts[k].transform.position.x < -Wco)
                {
                    nLeft++;
                }
                ghostLap[k] = !ghostLap[k];
            }
        }
    }
}
