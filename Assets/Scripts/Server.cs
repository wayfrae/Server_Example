using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    byte error;
    private const int BYTE_SIZE = 1440;
    private byte reliableChannel, unreliableChannel;
    private const int MAX_USER = 10;
    private int hostId, webHostId;

    private bool isStarted;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();

    }

    private void Update()
    {
        UpdateMessagePump();
    }

    public void UpdateMessagePump()
    {
        if (!isStarted)
        {
            return;
        }

        int recHostId;
        int connectionId;
        int channelId;

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

        switch (type)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User: {0} has connected through host {1}", connectionId, recHostId));
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("User: {0} has disconnected!", connectionId));
                break;
            case NetworkEventType.DataEvent:
                Debug.Log(string.Format("Data was received from client {0}.", connectionId));
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream(recBuffer);
                GameMessage message = (GameMessage)formatter.Deserialize(memoryStream);
                OnData(connectionId, channelId, recHostId, message);
                break;
            case NetworkEventType.BroadcastEvent:
            default:
                Debug.Log("Unexpected Network Event Type");
                break;
        }
    }

    private void OnData(int connId, int channelId, int recHostId, GameMessage message)
    {
        switch (message.Code)
        {
            case OperationCode.None:
                break;
            case OperationCode.CreateAccount:
                CreateAccount(connId, channelId, recHostId, (Message_CreateAccount)message);
                break;
            case OperationCode.Move:
                break;
            case OperationCode.Shoot:
                break;
            case OperationCode.Spawn:
                SpawnPlayer(connId, channelId, recHostId, (Message_Spawn)message);
                ShowHazards(connId, channelId, recHostId, (Message_Spawn)message);
                break;
        }
    }

    private void ShowHazards(int connId, int channelId, int recHostId, Message_Spawn message)
    {
        var gameControllerObj = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        StringBuilder s = new StringBuilder();
        s.Append("[");
        for(int i = 0; i < gameControllerObj.hazards.Count; i++)
        {
            s.Append(JsonUtility.ToJson(gameControllerObj.hazards[i]));
            if(i < gameControllerObj.hazards.Count - 1)
            {
                s.Append(",");
            }
        }
        s.Append("]");
        var messageHazard = new Message_Hazards
        {
            hazards = s.ToString()
        };
        SendClient(recHostId, connId, messageHazard);
    }

    private void SpawnPlayer(int connId, int channelId, int recHostId, Message_Spawn message)
    {
        //Debug.Log(string.Format("Player {2} position is X: {0}, Y: {1}", message.X, message.Y, connId));
        
    }

    

    private void CreateAccount(object cnnId, int channelId, object recHostId, Message_CreateAccount message)
    {
        Debug.Log(string.Format("{0}, {1}, {2}", message.Username, message.Password, message.Email));
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public void Init()
    {

        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.ReliableFragmented);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        cc.PacketSize = 1440;
        cc.FragmentSize = 900;
        HostTopology hostTopology = new HostTopology(cc, MAX_USER);

        hostId = NetworkTransport.AddHost(hostTopology, PORT, null);
        webHostId = NetworkTransport.AddWebsocketHost(hostTopology, WEB_PORT, null);

        Debug.Log(string.Format("Opening connection on port {0} and web_port {1}.", PORT, WEB_PORT));
        isStarted = true;

    }

    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    #region Send
    public void SendClient(int recHost, int connId, GameMessage message)
    {
        int actualHostId;
        byte[] buffer = new byte[BYTE_SIZE];
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        formatter.Serialize(memoryStream, message);
        if(recHost == 0)
        {
            actualHostId = hostId;
        }
        else
        {
            actualHostId = webHostId;
        }
        var serializedObject = memoryStream.ToArray();
        NetworkTransport.Send(actualHostId, connId, reliableChannel, serializedObject, BYTE_SIZE, out error);
    }
    #endregion
}
