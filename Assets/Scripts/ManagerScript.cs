using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class ManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spectatorPrefab;
    public GameObject spherePrefab;
    public GameObject lowResSpherePrefab;
    public GameObject emptyPrefab;
    public GameObject corridor;
    public GameObject closedRoom;

    public float corridorSize;

    private bool connect;
    private bool firstConnected;
    private bool connected;
    public bool disconnected;
    private bool spectator;
    private int playerType;
    private int playerIdx;
    public float Speed;
    public int level;
    public int nNeighbors;

    private TcpClient clientSocket;
    private UdpClient udpSend;
    private UdpClient udpReceive;
    private byte[] codeReadBuffer = new byte[1];
    private List<byte> codeSend = new List<byte>();
    private List<byte> codeSendUdp = new List<byte>();

    private List<GameObject> objects = new List<GameObject>();
    private List<PlayerNetwork> players = new List<PlayerNetwork>();

 

    public List<int> listId = new List<int>();

    public List<int> listScore = new List<int>();
    public List<string> listName = new List<string>();
    public List<float> listDistance = new List<float>();
    public List<int> removePlayer = new List<int>();
    FileStream fs;
    string paramPath;

    private double X;
    private double Y;
    private double Z;
    private double offsetZ;
    private double RX;
    private double RY;
    private double RZ;
    public int nPlayers;
    private int clientId;
    public string nameClient;
    public string host;
    public int port;
    private IPEndPoint ipRemoteEndPointReceive;
    private IPEndPoint ipRemoteEndPointSend;
    private ManagerScript managerScript;
    Thread reception;
    Thread sending;
    private bool sender;
    private bool receiver;

    private int nLeft;
    private int nRight;

    private byte[] Receivebuffer(int size)
    {
        NetworkStream stream = clientSocket.GetStream();
        byte[] array = new byte[size];
        stream.Read(array, 0, size);
        return array;
    }

    private void register()
    {
        
        byte code = 10;
        int byteCount = Encoding.UTF8.GetByteCount(nameClient);
        byte[] bytes = BitConverter.GetBytes(playerType);
        byte[] bytes2 = BitConverter.GetBytes(byteCount);
        byte[] bytes3 = Encoding.UTF8.GetBytes(nameClient);
        byte[] array = Combine(code, bytes, bytes2, bytes3);
        SendBuffer(array);
        codeReadBuffer = Receivebuffer(sizeof(Byte));
        

        //MR 

        if (codeReadBuffer[0] == 17)
        {
            byte[] array2 = Receivebuffer(sizeof(Double) * 1 + sizeof(Int32) * 2);
            PlayerPrefs.SetFloat("Speed", (float)BitConverter.ToDouble(array2, 0));
            PlayerPrefs.SetInt("Level", BitConverter.ToInt32(array2, sizeof(Double)));
            PlayerPrefs.SetInt("nNeighbors", BitConverter.ToInt32(array2, sizeof(Double)+sizeof(Int32)));

            Speed = PlayerPrefs.GetFloat("Speed");
            level = PlayerPrefs.GetInt("Level");
            nNeighbors = PlayerPrefs.GetInt("nNeighbors");
            Receivebuffer(sizeof(Byte));
            Debug.Log(17);
        }
        codeReadBuffer = Receivebuffer(sizeof(Byte));
        if (codeReadBuffer[0] == 19)
        {
            byte[] array2 = Receivebuffer(sizeof(Double) * 7);
            PlayerPrefs.SetFloat("Aco", (float)BitConverter.ToDouble(array2, 0));
            PlayerPrefs.SetFloat("Len", (float)BitConverter.ToDouble(array2, sizeof(Double)));
            PlayerPrefs.SetFloat("Lex", (float)BitConverter.ToDouble(array2, sizeof(Double) * 2));
            PlayerPrefs.SetFloat("Lco", (float)BitConverter.ToDouble(array2, sizeof(Double) * 3));
            PlayerPrefs.SetFloat("Wco", (float)BitConverter.ToDouble(array2, sizeof(Double) * 4));
            PlayerPrefs.SetFloat("Wbi", (float)BitConverter.ToDouble(array2, sizeof(Double) * 5));
            PlayerPrefs.SetFloat("Hco", (float)BitConverter.ToDouble(array2, sizeof(Double) * 6));

            Speed = PlayerPrefs.GetFloat("Speed");
            level = PlayerPrefs.GetInt("Level");
            nNeighbors = PlayerPrefs.GetInt("nNeighbors");
            Receivebuffer(sizeof(Byte));
            Debug.Log(17);
        }
        if (spectator)
        {
            paramPath = pathDefine("Score");
            if (!File.Exists(paramPath))
            {
                fs = File.Create(paramPath);
                fs.Close();
            }
        }

        switch(level)
        {
            case 11:
                Instantiate(corridor, new Vector3(0, 0, 0),new Quaternion(0,0,0,0));
                corridorSize = PlayerPrefs.GetFloat("Ltot");
                
                
                break;
            case 21:
                Instantiate(closedRoom, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                corridorSize = 200;
                break;


        }
    }

    private void send()
    {
        while (sender)
        {
            Thread.Sleep(100);
            //Debug.Log((20).ToString());
            codeSend.ForEach(delegate (byte code)
            {

                switch (code)
                {
                    case 30:

                        byte[] byteId = new byte[sizeof(Int32)];
                        byteId = BitConverter.GetBytes(clientId);
                        byte[] array2 = Combine(code,byteId);
                        SendBuffer(array2);
                        break;

                }
            });


            byte[][] message = new byte[7][];
            message[0] = BitConverter.GetBytes(clientId);
            message[1] = BitConverter.GetBytes(X);
            message[2] = BitConverter.GetBytes(Y);
            message[3] = BitConverter.GetBytes(Z);
            message[4] = BitConverter.GetBytes(RX);
            message[5] = BitConverter.GetBytes(RY);
            message[6] = BitConverter.GetBytes(RZ);
            SendBufferUdp((byte)20, Combine(message));



            codeSendUdp.Clear();
        }
    }
    private void receive()
    {
        while (receiver)
        {


            byte code = 0; 

            try
            {
                byte[] ReadBuffer = ReceivebufferUdp();

                code = ReadBuffer[0];
                switch (code)
                {

                    case 21: // list of positions



                        for (int i = 0; i < nPlayers; i++)
                        {

                            players[i].newPos(BitConverter.ToDouble(ReadBuffer, sizeof(Double) * i * 3 + 1),
                                                BitConverter.ToDouble(ReadBuffer, sizeof(Double) * (i * 3 + 1) + 1),
                                                BitConverter.ToDouble(ReadBuffer, sizeof(Double) * (i * 3 + 2) + 1));
                            /*
                        xPlayer [i] = BitConverter.ToDouble (value, sizeof(Double) * i * 3);
                        yPlayer [i] = BitConverter.ToDouble (value, sizeof(Double) * (i * 3 + 1));
                        zPlayer [i] = BitConverter.ToDouble (value, sizeof(Double) * (i * 3 + 2));
                        */
                        }

                        break;
                    case 41: // number of people in each corridor
                        nLeft = BitConverter.ToInt32(ReadBuffer, 1);
                        nRight = BitConverter.ToInt32(ReadBuffer, 1+sizeof(Int32));
                        break; 
                    case 31:
                        receiver = !receiver;
                        Application.Quit();
                        break;


                    case 33:

                        listId.Add(BitConverter.ToInt32(ReadBuffer, 1));
                        players.Add(new PlayerNetwork(listId.Last(), BitConverter.ToDouble(ReadBuffer, sizeof(Int32) + 1),
                                                        BitConverter.ToDouble(ReadBuffer, sizeof(Int32) + sizeof(Double) + 1),
                                                        BitConverter.ToDouble(ReadBuffer, sizeof(Int32) + sizeof(Double) * 2 + 1),
                                                        Speed));
                        listDistance.Add(10000);

                        break;

                    case 35:
                        byte[] nameBuffer = new byte[ReadBuffer.Length - 1];
                        Buffer.BlockCopy(ReadBuffer, 1, nameBuffer, 0, nameBuffer.Length);
                        listName.Add(Encoding.UTF8.GetString(nameBuffer));
                        listScore.Add(0);

                        break;
                    case 37:
                        int removeId = BitConverter.ToInt32(ReadBuffer, 1);
                        removePlayer.Add(removeId);



                        break;
                    case 39:
                        Debug.Log("h");
                        receiver = !receiver;
                        disconnect();
                        Application.Quit();
                        break;
                    case 23:
                        for (int i = 0; i < nPlayers; i++)
                        {

                            listScore[i] = BitConverter.ToInt32(ReadBuffer, sizeof(Int32) * i + 1);
                        }
                        break;
                    case 25:
                        receiver = !receiver;
                        disconnected = !disconnected;
                        codeSendUdp.Clear();
                        codeSend.Clear();

                        break;
                }
            }
            catch (Exception ex)
            {


            }

        }

    }
   
    private void RemoveObject(int idxPlayer)
    {
        //Debug.Log("listId : " + listId.Count());
        //Debug.Log("objects : " + objects.Count());
        //Debug.Log("idxPlayer : " + idxPlayer);

        Destroy(objects[idxPlayer]);
        listId.RemoveAt(idxPlayer);
        listDistance.RemoveAt(idxPlayer);
        players.RemoveAt(idxPlayer);

        if (spectator)
        {
            string appendText = listName[idxPlayer] + "\t" +
                listScore[idxPlayer] + "\t" +
                Environment.NewLine;
            File.AppendAllText(paramPath, appendText);
            fs.Dispose();
            listName.RemoveAt(idxPlayer);
            listScore.RemoveAt(idxPlayer);
        }

        objects.RemoveAt(idxPlayer);
        
        nPlayers--;
    }

   

    private void findPlayerIndex()
    {
        playerIdx = listId.IndexOf(clientId);
    }
    private int findPlayerIndex(int Id)
    {
        int Idx = listId.IndexOf(Id);
        return Idx;
    }

    private void codeWrite()
    {
        codeSendUdp.Add(20);
    }

    private void readListIdPlayer()
    {
        codeReadBuffer = Receivebuffer(sizeof(Byte));

        if (codeReadBuffer[0] == 11)
        {
            byte[] array = Receivebuffer(sizeof(Int32) * 2);
            nPlayers = BitConverter.ToInt32(array, 0);
            clientId = BitConverter.ToInt32(array, sizeof(Int32));
            byte[] value = Receivebuffer(sizeof(Int32) * nPlayers);
            for (int i = 0; i < nPlayers; i++)
            {
                listId.Add(BitConverter.ToInt32(value, sizeof(Int32) * i));
                listDistance.Add(10000);
            }
            Receivebuffer(sizeof(Byte));
        }


        if (spectator)
        {
            codeReadBuffer = Receivebuffer(sizeof(Byte));
            if (codeReadBuffer[0] == 13)
            {

                for (int j = 0; j < nPlayers; j++)
                {
                    int size = BitConverter.ToInt32(Receivebuffer(sizeof(Int32)), 0);
                    byte[] bytes = Receivebuffer(size);
                    listName.Add(Encoding.UTF8.GetString(bytes));
                    listScore.Add(0);
                }
                Receivebuffer(sizeof(Byte));
            }
        }

        codeReadBuffer = Receivebuffer(sizeof(Byte));

        if (codeReadBuffer[0] == 21)
        {
            byte[] value2 = Receivebuffer(nPlayers * sizeof(Double) * 3);
            for (int k = 0; k < nPlayers; k++)
            {
                players.Add(new PlayerNetwork(listId[k],
                                                BitConverter.ToDouble(value2, sizeof(Double) * k * 3),
                                                BitConverter.ToDouble(value2, sizeof(Double) * (k * 3 + 1)),
                                                BitConverter.ToDouble(value2, sizeof(Double) * (k * 3 + 2)),
                                                Speed));
                /*
				xPlayer.Add (BitConverter.ToDouble (value2, sizeof(Double) * k * 3));
				yPlayer.Add (BitConverter.ToDouble (value2, sizeof(Double) * (k * 3 + 1)));
				zPlayer.Add (BitConverter.ToDouble (value2, sizeof(Double) * (k * 3 + 2)));
				*/
            }
            Receivebuffer(sizeof(Byte));
        }
    }

    private void generateWorld()
    {
        findPlayerIndex();
        for (int i = 0; i < nPlayers; i++)
        {
            if (i == playerIdx)
            {
                X = players[i].X;
                Y = players[i].Y;
                Z = players[i].Z+offsetZ;
                RX = 0;
                RY = 0;
                RZ = 0;

                
            }
            Spawn(players[i]);
        }
    }

    private void updateWorld()
    {
        findPlayerIndex();

        X = (double)objects[playerIdx].transform.position.x;
        Y = (double)objects[playerIdx].transform.position.y;
        if (level==11 && objects[playerIdx].transform.position.z > 0)
        {
            objects[playerIdx].transform.Translate(new Vector3(0, 0, -corridorSize));
            offsetZ += corridorSize;
            Debug.Log(corridorSize);
        }
        Z = (double)objects[playerIdx].transform.position.z+offsetZ;
        RX = (double)objects[playerIdx].transform.rotation.x;
        RY = (double)objects[playerIdx].transform.rotation.y;
        RZ = (double)objects[playerIdx].transform.rotation.z;
        for (int i = 0; i < nPlayers; i++)
        {
            

            players[i].positionUpdate(objects[i].transform.position);
            if (i != playerIdx)
            {

                //objects[i].transform.position=players[i].posPlayer.Last();
                objects[i].transform.Translate(players[i].target * Time.deltaTime * players[i].Speed);
                
                if (level==11 && objects[i].transform.position.z > 0)
                {
                    objects[i].transform.Translate(new Vector3(0, 0, -corridorSize));

                }
                else if(level>20)
                {
                    listDistance[i]=(objects[i].transform.position-objects[playerIdx].transform.position).magnitude;
                }
                //objects [i].transform.position = Vector3.MoveTowards(objects [i].transform.position,players[i].target, 1);
            }

        }
        if (level > 20)
        {


            var sorted = listDistance
                .Select((x, i) => new KeyValuePair<float, int>(x, i))
                .OrderBy(x => x.Key)
                .ToList();

            List<int> idx = sorted.Select(x => x.Value).ToList();
            for (int k = 0; k < idx.Count(); k++)
            {
                if(k != playerIdx)
                {
                    if (k < nNeighbors)
                    {
                        objects[k].GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        objects[k].GetComponent<MeshRenderer>().enabled = true;
                    }
                }
                
            }
        }
        
    }

    private void Spawn(PlayerNetwork player)
    {
        int id = player.clientId;

        Vector3 position = player.posPlayer.First();
        if (id != clientId)
        {
            if (id < 1000)
            {
                
                if (!spectator)
                {
                    
                    objects.Add((GameObject)UnityEngine.Object.Instantiate(lowResSpherePrefab, position, Quaternion.identity));
                }
                else
                {
                    objects.Add((GameObject)UnityEngine.Object.Instantiate(spherePrefab, position, Quaternion.identity));
                }
            }

            else
            {
                objects.Add((GameObject)UnityEngine.Object.Instantiate(emptyPrefab, position, Quaternion.identity));

            }
        }
        else if (!spectator)
        {
                
                objects.Add((GameObject)UnityEngine.Object.Instantiate(playerPrefab, position, Quaternion.identity));


        }
        else
        {
            objects.Add((GameObject)UnityEngine.Object.Instantiate(spectatorPrefab, position, Quaternion.identity));
        }
    }



    private void Start()
    {

        nLeft = 0;
        nRight = 0;
        sender = false;
        receiver = false;
        connect = true;
        connected = false;
        firstConnected = false;
        disconnected = false;
#if UNITY_ANDROID
        spectator = false;
#else
        spectator = true;
#endif
        playerType = Convert.ToInt32(spectator);
        nameClient = "abruti";
        clientSocket = new TcpClient();
        udpSend = new UdpClient();
        udpReceive = new UdpClient();
        udpReceive.Client.ReceiveTimeout = 100;
        udpSend.Client.SendTimeout = 100;
        reception = new Thread(receive);
        sending = new Thread(send);

    }

    private void Update()
    {
        PlayerPrefs.SetInt("nLeft", nLeft);
        PlayerPrefs.SetInt("nRight", nRight);
        if (disconnected)
        {

            Debug.Log(disconnected);
            playerType = 2;
            int Nplayers= objects.Count;
            for (int k = 0; k < Nplayers; k++)
            {
                RemoveObject(0);

            }
            disconnected = !disconnected;
            connect = !connect;
            disconnect();
            codeSend.Clear();
            codeSendUdp.Clear();
            removePlayer.Clear();
            clientSocket = new TcpClient();
            udpSend = new UdpClient();
            udpReceive = new UdpClient();
            udpReceive.Client.ReceiveTimeout = 100;
            udpSend.Client.SendTimeout = 100;
            reception = new Thread(receive);
            sending = new Thread(send);
            Thread.Sleep(1000);

        }
        if (!connected)
        {

            if (connect)
            {
                connect = !connect;


                Debug.Log("Name = " + nameClient);
                Debug.Log("Host = " + host);
                Debug.Log("Port = " + port);
                try
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(host), port);
                    ipRemoteEndPointReceive = new IPEndPoint(IPAddress.Parse(host), port + 2);
                    ipRemoteEndPointSend = new IPEndPoint(IPAddress.Parse(host), port + 1);

                    clientSocket.Connect(remoteEP);
                    udpReceive = new UdpClient(port + 2);
                    udpSend = new UdpClient();
                    clientSocket.NoDelay = true;
                    Debug.Log("Name = " + nameClient);
                    udpSend.Connect(ipRemoteEndPointSend);
                    register();
                    Debug.Log("Name = " + nameClient);
                    readListIdPlayer();
                    generateWorld();
                    if (!firstConnected)
                    {

                        firstConnected = !firstConnected;
                    }
                    connected = !connected;
                    receiver =!receiver;
                    sender = !sender;
                    sending.Start();
                    reception.Start();
                    
                }
                catch (Exception message)
                {
                    Debug.Log(message);
                }
            }

        }
        else
        {
            if (removePlayer.Any())

            {

                for (int l = 0; l < removePlayer.Count(); l++)
                {
                    int removeId = removePlayer[l];
                    int removeIdx = findPlayerIndex(removeId);
                    RemoveObject(removeIdx);
                    if(removeId==clientId)
                    {
                        receiver = !receiver;
                        disconnect();

                        Application.Quit();
                    }
                }
                removePlayer.Clear();
            }
            codeWrite();
            findPlayerIndex();
            X = (double)objects[playerIdx].transform.position.x;
            Y = (double)objects[playerIdx].transform.position.y;
            if (level==11 && objects[playerIdx].transform.position.z > 0)
            {
                objects[playerIdx].transform.Translate(new Vector3(0, 0, -corridorSize));
                offsetZ += corridorSize;
            }
            Z = (double)objects[playerIdx].transform.position.z+offsetZ;

            //send();
            if (players.Count > nPlayers) //PEnser à regarder ça en détail
            {
                Spawn(players.Last());
                nPlayers++;
            }
            //receive ();
            updateWorld();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(connected)
            {
                receiver = !receiver;
                disconnect();
            
                Application.Quit();
            }
            else
            {
                Application.Quit();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {


            byte[] code24 = new byte[1];
            code24[0] = 24;
            SendBufferUdp(code24);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {


            byte[] code26 = new byte[1];
            code26[0] = 26;
            SendBufferUdp(code26);
        }
    }
    private void disconnect()
    {
        sender = !sender;
        Debug.Log(disconnected);
        SendBufferUdp((byte)30, BitConverter.GetBytes(clientId));

        clientSocket.Close();
        udpSend.Close();
        udpReceive.Close();

        if (spectator)
        {
            for (int k = 0; k < listName.Count; k++)
            {
                string appendText = listName[k] + "\t" +
                    listId[k] + "\t" +
                    listScore[k] + "\t" +
                    Environment.NewLine;
                File.AppendAllText(paramPath, appendText);
                fs.Dispose();
            }


        }
        connected = !connected;
        sending.Abort();
        reception.Abort();
    }

    private void SendBuffer(byte[] buf)
    {
        try
        {
            byte[] array = new byte[1 + buf.Length];
            Buffer.BlockCopy(buf, 0, array, 0, buf.Length);
            array[buf.Length] = 255;
            NetworkStream stream = clientSocket.GetStream();
            stream.Write(array, 0, array.Length);
            stream.Flush();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private void SendBufferUdp(byte[] buf)
    {
        try
        {
            byte[] array = new byte[1 + buf.Length];
            Buffer.BlockCopy(buf, 0, array, 0, buf.Length);
            array[buf.Length] = 255;
            udpSend.Send(array, array.Length);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private void SendBufferUdp(byte code, byte[] buf)
    {
        try
        {
            byte[] array = new byte[2 + buf.Length];
            array[0] = code;
            Buffer.BlockCopy(buf, 0, array, 1, buf.Length);
            array[buf.Length + 1] = 255;
            udpSend.Send(array, array.Length);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private byte[] ReceivebufferUdp()
    {
        byte[] array;
        try
        {

            array = udpReceive.Receive(ref ipRemoteEndPointReceive);

        }
        catch (Exception ex)
        {
            array = new byte[1];
            array[0] = 254;

        }
        return array;
    }

    private byte[] Combine(byte code)
    {
        byte[] array = new byte[1];
        array[0] = code;
        return array;
    }
    private byte[] Combine(byte code, byte[] first)
    {
        byte[] array = new byte[1 + first.Length];
        array[0] = code;
        Buffer.BlockCopy(first, 0, array, 1, first.Length);
        return array;
    }
    private byte[] Combine(byte code, byte[] first, byte[] second)
    {
        byte[] array = new byte[1 + first.Length + second.Length];
        array[0] = code;
        Buffer.BlockCopy(first, 0, array, 1, first.Length);
        Buffer.BlockCopy(second, 0, array, 1 + first.Length, second.Length);
        return array;
    }
    private byte[] Combine(byte code, byte[] first, byte[] second, byte[] third)
    {
        byte[] array = new byte[1 + first.Length + second.Length + third.Length];
        array[0] = code;
        Buffer.BlockCopy(first, 0, array, 1, first.Length);
        Buffer.BlockCopy(second, 0, array, 1 + first.Length, second.Length);
        Buffer.BlockCopy(third, 0, array, 1 + first.Length + second.Length, third.Length);
        return array;
    }
    private byte[] Combine(byte code, params double[] var)
    {
        byte[] array = new byte[1 + var.Length * 8];
        int num = 1;
        array[0] = code;
        for (int i = 0; i < var.Length; i++)
        {
            double value = var[i];
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, array, num, 8);
            num += 8;
        }
        return array;
    }
    private byte[] Combine(params byte[][] arrays)
    {
        byte[] array = new byte[arrays.Sum((byte[] x) => x.Length)];
        int num = 0;
        for (int i = 0; i < arrays.Length; i++)
        {
            byte[] array2 = arrays[i];
            Buffer.BlockCopy(array2, 0, array, num, array2.Length);
            num += array2.Length;
        }
        return array;
    }

    string pathDefine(string namePath)
    {
        string path1 = "../Levels";
        string path2 = path1 + @"/" + level.ToString();
        string path3 = path2 + @"/" + DateTime.Today.ToString("s").Substring(2, 8);
        string path4 = path3 + @"/" + namePath;
        string fullpath = path4 + @"/" + DateTime.Now.ToString("s").Substring(11, 2) + "-" +
            DateTime.Now.ToString("s").Substring(14, 2) + "-" +
                DateTime.Now.ToString("s").Substring(17, 2) + ".txt";

        if (!Directory.Exists(path1))
        {
            Directory.CreateDirectory(path1);
        }
        if (!Directory.Exists(path2))
        {
            Directory.CreateDirectory(path2);
        }
        if (!Directory.Exists(path3))
        {
            Directory.CreateDirectory(path3);
        }
        if (!Directory.Exists(path4))
        {
            Directory.CreateDirectory(path4);
        }
        return fullpath;
    }
}
