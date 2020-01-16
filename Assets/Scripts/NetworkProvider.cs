using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class NetworkProvider : MonoBehaviour
{
    public bool isServer = false;
    public bool debug = true;
    public string ip = "127.0.0.1";
    public ushort port = 15937;

    private NetworkManager mgr;
    public GameObject networkManager;

    public int maxPlayers = 10;

    private NetWorker server;
    private UDPClient client;

    private LogHandler _logger;

    // Start is called before the first frame update
    void Start()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        Rpc.MainThreadRunner = MainThreadManager.Instance;

        _logger = LogHandler.Instance;

        if (isServer)
        {
            //Server setup
            NetWorker.PingForFirewall(port);

            Host();
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
        ((UDPServer) server).Connect(ip, port);
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