using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshRoomGeneratorHardEdge : MonoBehaviour {

    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector3> newVertices2 = new List<Vector3>();
    public List<int> newTriangles2 = new List<int>();
    public List<float> xGrid = new List<float>();
    public List<float> yGrid = new List<float>();
    public List<float> zGrid = new List<float>();

    private Mesh mesh;

    float Len;
    float Lco;
    float Lex;
    float Lbi;
    float W;
    float Wbi;
    float H;
    float A;
    float Ltot;
     
    // Use this for initialization
    void Start() {
        mesh = GetComponent<MeshFilter>().mesh;


        W = 300;
        H = 3;

         
       

        Ltot = W;
        PlayerPrefs.SetFloat("Ltot",Ltot);

        room();


        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        ;
        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void triang(int x0, int x1, int x2)
    {
        int[] idx = new int[3];
        
        idx[0] = x0;
        idx[1] = x1;
        idx[2] = x2;


        newTriangles.Add(idx[0]);
        newTriangles.Add(idx[1]);
        newTriangles.Add(idx[2]);
    }
    void quad(int x0, int x1, int x2, int x3)
    {

        int[] idx = new int[4];
        idx[0] = x0;
        idx[1] = x1;
        idx[2] = x2;
        idx[3] = x3;
        


        newTriangles.Add(idx[0]);
        newTriangles.Add(idx[1]);
        newTriangles.Add(idx[3]);
        newTriangles.Add(idx[1]);
        newTriangles.Add(idx[2]);
        newTriangles.Add(idx[3]);
    }

    void room()
    {
        
        xGrid.Add(-W);
        xGrid.Add(W);
        

        yGrid.Add(0);
        yGrid.Add(H);


        zGrid.Add(-W);
        zGrid.Add(W);
        
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[0]));
        quad(0, 1, 2, 3);
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[0]));
        quad(7, 6, 5, 4);

        //Floor


        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[1]));
        quad(8, 9, 11, 10);
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[0]));
        quad(13, 12, 14, 15);
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[1]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[1]));
        quad(17, 18, 19, 16);
        newVertices.Add(new Vector3(xGrid[1], yGrid[1], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[1], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[0], yGrid[0], zGrid[0]));
        newVertices.Add(new Vector3(xGrid[1], yGrid[0], zGrid[0]));
        quad(23, 20, 21, 22);

    }
}
