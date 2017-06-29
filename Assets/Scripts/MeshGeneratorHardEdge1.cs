using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGeneratorHardEdge1 : MonoBehaviour {

    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<float> xGrid = new List<float>();
    public List<float> yGrid = new List<float>();
    public List<float> zGrid = new List<float>();

    public GameObject board1;
    public GameObject board2;

    private Mesh mesh;

    float Len;
    float Lco;
    float Lex;
    float Lbi;
    float W;
    float Wbi;
    float H;
    float A;
    float A1;
    float Ltot;
    int Nv;
    public bool ceiling;
    public bool light;
    GameObject[] pointLights = new GameObject[8];
    Light[] pointLightsComp = new Light[8];
    // Use this for initialization
    void Start() {
        ceiling = false;
        light = false;
        Nv = 0;
        mesh = GetComponent<MeshFilter>().mesh;
        A1 = PlayerPrefs.GetFloat("Aco");
        Len = PlayerPrefs.GetFloat("Len");
        Lex = PlayerPrefs.GetFloat("Lex");
        Lco = PlayerPrefs.GetFloat("Lco");
        W = PlayerPrefs.GetFloat("Wco");
        Wbi = PlayerPrefs.GetFloat("Wbi");
        H = PlayerPrefs.GetFloat("Hco");

        A = Mathf.PI / 6;
        Debug.Log(A);
        Debug.Log(Len);
        Debug.Log(Lex);
        Lbi = (Wbi / Mathf.Tan(A)) - (Wbi - W / 2) / Mathf.Tan(A);

        Ltot = Len + Lex + Lco +Wbi / Mathf.Tan(A)+ Wbi / Mathf.Tan(A1);

        PlayerPrefs.SetFloat("Ltot",Ltot);

        simpleCorridor(0);
        xGrid.Clear();
        yGrid.Clear();
        zGrid.Clear();
        Nv = newVertices.Count;
        simpleCorridor(1);
        xGrid.Clear();
        yGrid.Clear();
        zGrid.Clear();
        simpleCorridor(2);
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh=mesh;
        if (light)
        {
            for (int k = 0; k < pointLights.Length; k++)
            {
                pointLights[k] = new GameObject();
                pointLightsComp[k] = pointLights[k].AddComponent<Light>();
                pointLightsComp[k].type = LightType.Point;

                if (k % 3 == 2)
                {
                    pointLightsComp[k].range = Ltot / 2;
                    pointLightsComp[k].intensity = 1f;
                }
                else if (k % 3 == 1)
                {
                    pointLightsComp[k].range = Ltot / 5;
                    pointLightsComp[k].color = new Color(0, 255, 0);
                    pointLights[k].AddComponent<lightColorTrail>();
                    pointLights[k].GetComponent<lightColorTrail>().left = true;
                    pointLightsComp[k].intensity = 8f;
                }
                else
                {

                    pointLightsComp[k].range = Ltot / 5;
                    pointLightsComp[k].color = new Color(255, 0, 0);
                    pointLights[k].AddComponent<lightColorTrail>();
                    pointLights[k].GetComponent<lightColorTrail>().left = false;
                    pointLightsComp[k].intensity = 8f;
                }


            }
        }
        int j = -1;
        float wa = Ltot / 4;
        j++;
        pointLights[j].transform.position = new Vector3(Wbi, W / 2, + Ltot / 2 - wa);
        j++;
        pointLights[j].transform.position = new Vector3(-Wbi, W / 2, + Ltot / 2 - wa);
        j++;
        pointLights[j].transform.position = new Vector3(0, W/2, 0);
        j++;
        pointLights[j].transform.position = new Vector3(Wbi, W / 2, -Ltot / 2 - wa);
        j++;
        pointLights[j].transform.position = new Vector3(-Wbi, W / 2, -Ltot / 2 - wa);
        j++;
        pointLights[j].transform.position = new Vector3(0, W / 2, -Ltot);
        j++;
        pointLights[j].transform.position = new Vector3(Wbi, W / 2, -3*Ltot / 2 - wa);
        j++;
        pointLights[j].transform.position = new Vector3(-Wbi, W / 2, -3*Ltot / 2 - wa);

        board1.transform.position = new Vector3(0, H/2, Len-Ltot);
        board2.transform.position = new Vector3(0, H/2, Len);
    }

    // Update is called once per frame
    void Update () {
	
	}
    void triang(int x0, int x1, int x2,int pos)
    {
        int[] idx = new int[3];
        int idx0 = Nv * pos;
        idx[0] = x0 + idx0;
        idx[1] = x1 + idx0;
        idx[2] = x2 + idx0;
        /*
        for (int k=0;k<3;k++)
        {
            if (idx[k] == 72) idx[k] = 15;
            if (idx[k] == 39) idx[k] = 16;
            if (idx[k] == 72 + 18) idx[k] = 33;
            if (idx[k] == 72 + 21) idx[k] = 34+72;

            if (idx[k] == 72+72) idx[k] = 15 + 72;
            if (idx[k] == 39+72) idx[k] = 16 + 72;
            if (idx[k] == 72 + 18+72) idx[k] = 33 + 72;
            if (idx[k] == 72 + 21+72) idx[k] = 34 + 72;

        }
        */

        newTriangles.Add(idx[0]);
        newTriangles.Add(idx[1]);
        newTriangles.Add(idx[2]);
    }
    void quad(int x0, int x1, int x2, int x3,int pos)
    {

        int[] idx = new int[4];
        int idx0 = Nv * pos;
        idx[0] = x0 + idx0;
        idx[1] = x1 + idx0;
        idx[2] = x2 + idx0;
        idx[3] = x3 + idx0;
        
        for (int k = 0; k < 4; k++)
        {
            if (idx[k] == 36) idx[k] = 15;
            if (idx[k] == 39) idx[k] = 16;
            if (idx[k] == 36 + 18) idx[k] = 33;
            if (idx[k] == 36 + 21) idx[k] = 34;

            if (idx[k] == 36 + Nv) idx[k] = 15 + Nv;
            if (idx[k] == 39 + Nv) idx[k] = 16 + Nv;
            if (idx[k] == 36 + 18 + Nv) idx[k] = 33 + Nv;
            if (idx[k] == 36 + 21 + Nv) idx[k] = 34 + Nv;

        }
        
        newTriangles.Add(idx[0]);
        newTriangles.Add(idx[1]);
        newTriangles.Add(idx[3]);
        newTriangles.Add(idx[1]);
        newTriangles.Add(idx[2]);
        newTriangles.Add(idx[3]);
    }

    void quad2(int x0, int x1, int x2, int x3, int pos)
    {
        int[] idx = new int[4];
        int idx0 = Nv * pos;
        idx[0] = x0 + idx0;
        idx[1] = x1 + idx0;
        idx[2] = x2 + idx0;
        idx[3] = x3 + idx0;
        for (int k=0;k<4;k++)
        {
            newVertices.Add(newVertices[idx[k]]);
        }
        int N = newVertices.Count;

        newTriangles.Add(N - 4);
        newTriangles.Add(N - 3);
        newTriangles.Add(N - 1);
        newTriangles.Add(N - 3);
        newTriangles.Add(N - 2);
        newTriangles.Add(N - 1);
        

    }

    void simpleCorridor(int pos)
    {
        float z0 = -pos * Ltot;
        xGrid.Add(-W / 2 - Wbi);
        xGrid.Add(-W / 2 - Wbi + W);
        xGrid.Add(-W / 2);
        xGrid.Add(0);
        xGrid.Add(W / 2);
        xGrid.Add(+W / 2 + Wbi - W);
        xGrid.Add(+W / 2 + Wbi);


        yGrid.Add(0);
        yGrid.Add(H);


        zGrid.Add(Len + Lex + Lco +  Wbi / Mathf.Tan(A) + Wbi / Mathf.Tan(A1) + z0);
        zGrid.Add(Lex + Lco +  Wbi / Mathf.Tan(A) + Wbi / Mathf.Tan(A1) + z0);
        zGrid.Add(-Lbi + Lex + Lco +  Wbi / Mathf.Tan(A) + Wbi / Mathf.Tan(A1) + z0);
        zGrid.Add(Lex + Lco + Wbi / Mathf.Tan(A1) + z0);
        zGrid.Add(Lex + Wbi / Mathf.Tan(A1) + z0);
        zGrid.Add(Lbi + Lex + z0);
        zGrid.Add(Lex + z0);
        zGrid.Add(0 + z0);


        zGrid.Add(zGrid[2] - (Wbi - W / 2) / Mathf.Tan(A));
        zGrid.Add(Lbi + Lex + z0+ (Wbi - W / 2) / Mathf.Tan(A1));

        newVertices.Add(new Vector3(xGrid[4], yGrid[0], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[4], yGrid[0], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[0], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[0], zGrid[0]));

        newVertices.Add(new Vector3(xGrid[3], yGrid[0], zGrid[2]));

        newVertices.Add(new Vector3(xGrid[6], yGrid[0], zGrid[3]));
        newVertices.Add(new Vector3(xGrid[6], yGrid[0], zGrid[4]));
        newVertices.Add(new Vector3(xGrid[5], yGrid[0], zGrid[9]));
        newVertices.Add(new Vector3(xGrid[5], yGrid[0], zGrid[8]));

        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[8]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[9]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[4]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[3]));

        newVertices.Add(new Vector3(xGrid[3], yGrid[0], zGrid[5]));

        newVertices.Add(new Vector3(xGrid[4], yGrid[0], zGrid[6]));
        newVertices.Add(new Vector3(xGrid[4], yGrid[0], zGrid[7]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[0], zGrid[7]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[0], zGrid[6]));
        newVertices.Add(new Vector3(xGrid[4], yGrid[1], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[4], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[1], zGrid[0]));

        newVertices.Add(new Vector3(xGrid[3], yGrid[1], zGrid[2]));

        newVertices.Add(new Vector3(xGrid[6], yGrid[1], zGrid[3]));
        newVertices.Add(new Vector3(xGrid[6], yGrid[1], zGrid[4]));
        newVertices.Add(new Vector3(xGrid[5], yGrid[1], zGrid[9]));
        newVertices.Add(new Vector3(xGrid[5], yGrid[1], zGrid[8]));

        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[8]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[9]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[4]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[3]));

        newVertices.Add(new Vector3(xGrid[3], yGrid[1], zGrid[5]));

        newVertices.Add(new Vector3(xGrid[4], yGrid[1], zGrid[6]));
        newVertices.Add(new Vector3(xGrid[4], yGrid[1], zGrid[7]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[1], zGrid[7]));
        newVertices.Add(new Vector3(xGrid[2], yGrid[1], zGrid[6]));

        //Floor
        quad(0, 1, 2, 3, pos);
        quad(1, 5, 8, 4, pos);
        quad(4, 9, 12, 2, pos);
        quad(5, 6, 7, 8, pos);
        quad(9, 10, 11, 12, pos);
        quad(6, 14, 13, 7, pos);
        quad(10, 13, 17, 11, pos);
        quad(14, 15, 16, 17, pos);
        triang(1, 4, 2, pos);
        triang(13, 14, 17, pos);

        //Celing
        if (ceiling)
        {
            quad(18, 21, 20, 19, pos);
            quad(19, 22, 26, 23, pos);
            quad(22, 20, 30, 27, pos);
            quad(23, 26, 25, 24, pos);
            quad(30, 29, 28, 27, pos);
            quad(24, 25, 31, 32, pos);
            quad(28, 29, 35, 31, pos);
            quad(35, 34, 33, 32, pos);
            triang(19, 20, 22, pos);
            triang(31, 35, 32, pos);
        }
    
        //External Left Wall
        quad2(1, 0, 18, 19, pos);
        quad2(5, 1, 19, 23, pos);
        quad2(6, 5, 23, 24, pos);
        quad2(14, 6, 24, 32, pos);
        quad2(15, 14, 32, 33, pos);
        
        //External Right Wall
        quad2(3, 2, 20, 21, pos);
        quad2(2, 12, 30, 20, pos);
        quad2(12, 11, 29, 30, pos);
        quad2(11, 17, 35, 29, pos);
        quad2(17, 16, 34, 35, pos);

        //Internal Left Wall
        quad2(9, 4, 22, 27, pos);
        quad2(10, 9, 27, 28, pos);
        quad2(13, 10, 28, 31, pos);

        //INternal Right Wall
        quad2(4, 8, 26, 22, pos);
        quad2(8, 7, 25, 26, pos);
        quad2(7, 13, 31, 25, pos);



    }
}
