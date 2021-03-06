﻿using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using System.Net;

public class NetworkProvider : MonoBehaviour
{
    public bool isServer = false;
    public bool debug = true;
    public string ip = "127.0.0.1";
    public ushort port = 15937;

    public string apiIp = "62.210.180.72";
    public ushort apiPort = 61683;

    private NetworkManager mgr;
    public GameObject networkManager;

    public int maxPlayers = 10;

    private NetWorker server;
    private UDPClient client;

    private LogHandler _logger;
    private string publicIp;

    // Start is called before the first frame update
    void Start()
    {
        string[] arguments = Environment.GetCommandLineArgs();

        if (arguments.Length != 0)
        {
            for (var index = 0; index < arguments.Length; index++)
            {
                if (arguments[index] == "-ip")
                {
                    ip = arguments[index + 1];
                }
                else if (arguments[index] == "-port")
                {
                    port = ushort.Parse(arguments[index + 1]);
                }
            }
        }

        Rpc.MainThreadRunner = MainThreadManager.Instance;

        _logger = LogHandler.Instance;

        if (isServer)
        {
            publicIp = new WebClient().DownloadString("http://bot.whatismyipaddress.com");

            //Server setup
            NetWorker.PingForFirewall(port);
            Host();
            NotifyManager();
        }
        else
            Connect();
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void NotifyManager()
    {
        var client = new System.Net.Sockets.TcpClient();
        var ipendpoint = new IPEndPoint(IPAddress.Parse(apiIp), apiPort);
        client.Connect(ipendpoint);
        var message = new
            {MessageType = "ServerStarted", MessageData = new {ServerIP = publicIp, ServerPort = port}};
        var jsonMsg = "{ \"MessageType\": \"ServerStarted\", \"MessageData\": { \"ServerIP\": \"" + publicIp +
                      "\", \"ServerPort\": " + port + "}}";
        Debug.Log(jsonMsg.ToString());
        var stream = client.GetStream();
        var messageByteArray = Encoding.UTF8.GetBytes(jsonMsg);
        var messageLength = messageByteArray.Length;
        var messageLengthByteArray = new byte[4];
        messageLengthByteArray = BitConverter.GetBytes(messageLength);
        var fullMessage = new byte[messageLength + 4];
        Buffer.BlockCopy(messageLengthByteArray, 0, fullMessage, 0, messageLengthByteArray.Length);
        Buffer.BlockCopy(messageByteArray, 0, fullMessage, messageLengthByteArray.Length, messageByteArray.Length);
        stream.Write(fullMessage, 0, fullMessage.Length);
        stream.Flush();
        stream.Close();
        client.Close();
    }

    public void Host()
    {
        server = new UDPServer(maxPlayers);
        for (int i = 0; i < 5; i++)
        {
            try
            {
                ((UDPServer) server).Connect(ip, port);
                break;
            }
            catch (FailedBindingException)
            {
                port++;
            }
        }

        Connected(server);
    }

    public void Connect()
    {
        client = new UDPClient();
        client.Connect(ip, port);
        Connected(client);
    }

    public void Connected(NetWorker netWorker)
    {
        if (!netWorker.IsBound)
        {
            Debug.LogError("NetWorker failed to bind");
            return;
        }

        if (mgr == null && networkManager == null)
        {
            Debug.LogWarning("A network manager was not provided, generating a new one instead");
            networkManager = new GameObject("Network Manager");
            mgr = networkManager.AddComponent<NetworkManager>();
        }
        else if (mgr == null)
            mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

        mgr.Initialize(netWorker, null, 0);

        if (server is IServer)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Disconnect()
    {
        client.Disconnect(true);

        _logger.Log(LogTag.Network, new[] {Time.time.ToString(), "Disconnect server"});
    }

    private void OnApplicationQuit()
    {
        NetWorker.EndSession();

        server?.Disconnect(true);

        if (isServer)
            _logger.UploadLog("test");
    }
}