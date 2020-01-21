using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class GameManager : GameManagerBehavior
{
    private GameObject provider;
    private NetworkProvider networkProvider;
    private DungeonGenerator dungeonGenerator;

    public Guid gameID;
    public string gameIDString;

    // Start is called before the first frame update
    void Start()
    {
        provider = GameObject.Find("NetworkProvider");
        networkProvider = provider.GetComponent<NetworkProvider>();
        dungeonGenerator = GameObject.Find("Dungeon").GetComponent<DungeonGenerator>();

        if (networkProvider.isServer)
        {
            gameID = Guid.NewGuid();
            gameIDString = gameID.ToString();

            networkObject.SendRpc(RPC_SHARE_G_U_I_D, Receivers.Others, gameID.ToString());
        }

        dungeonGenerator.CallGenerate(gameID.GetHashCode());

        if (!networkProvider.isServer || networkProvider.debug)
        {
            var position = dungeonGenerator.PlayerSpawnPoints[
                UnityEngine.Random.Range(0, dungeonGenerator.PlayerSpawnPoints.Count)];

            NetworkManager.Instance.InstantiatePlayerController(position: new Vector3(position.x, 2, position.y));
        }
    }

    //RCPs
    public override void ShareGUID(RpcArgs args)
    {
        gameIDString = args.GetNext<string>();
        gameID = Guid.Parse(gameIDString);
    }
}