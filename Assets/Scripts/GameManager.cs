using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class GameManager : GameManagerBehavior
{
    private GameObject provider;
    private NetworkProvider networkProvider;
    
    // Start is called before the first frame update
    void Start()
    {
        provider = GameObject.Find("NetworkProvider");
        networkProvider = provider.GetComponent<NetworkProvider>();
        
        if(!networkProvider.isServer || networkProvider.debug)
            NetworkManager.Instance.InstantiatePlayerController(position: new Vector3(0, 2, 0));
    }

    private void spawnPlayer()
    {
        
    }
}