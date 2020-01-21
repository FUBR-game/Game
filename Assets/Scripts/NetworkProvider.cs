using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using System;
using System.Net.Sockets;
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

    // Start is called before the first frame update
    void Start()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        Rpc.MainThreadRunner = MainThreadManager.Instance;

        if (isServer)
        {
            var publicIp = new WebClient().DownloadString("http://bot.whatismyipaddress.com");

            //Server setup
            NetWorker.PingForFirewall(port);
            Host();
            var client = new System.Net.Sockets.TcpClient();
            var ipendpoint = new IPEndPoint(IPAddress.Parse(apiIp), apiPort);
            client.Connect(ipendpoint);
            var message = new { MessageType = "ServerStarted", MessageData = new { ServerIP = publicIp, ServerPort = port } };
            var jsonMsg = "{ \"MessageType\": \"ServerStarted\", \"MessageData\": { \"ServerIP\": \"" + publicIp + "\", \"ServerPort\": " + port + "}}";
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
        else
            Connect();
    }
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Host()
    {
        server = new UDPServer(maxPlayers);
        for(int i = 0; i < 5; i++)
        {
            try
            {
                ((UDPServer)server).Connect(ip, port);
                break;
            }
            catch(FailedBindingException ignored)
            {
                port++;
                continue;
            }
        }
        Connected(server);
    }

    public void Connect()
    {
        NetWorker client = new UDPClient();
        ((UDPClient) client).Connect(ip, port);
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

    private void OnApplicationQuit()
    {
        NetWorker.EndSession();

        server?.Disconnect(true);
    }
}